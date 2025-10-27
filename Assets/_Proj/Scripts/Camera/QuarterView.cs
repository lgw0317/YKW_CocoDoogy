using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchES = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// QuarterView
/// - 피벗(Tracking Target)을 기준으로 카메라의 궤도 회전(Yaw/Pitch) + 거리(줌)를 제어합니다.
/// - 마우스/한 손가락 드래그: 회전, 마우스 휠/핀치: 줌.
/// - 오브젝트 드래그 중(EditModeController.BlockOrbit)에는 "회전"만 차단(줌은 허용).
/// - CinemachineCamera + CinemachineFollow 조합에서 FollowOffset(y,z)만 조정합니다.
/// </summary>
public class QuarterView : MonoBehaviour
{
    #region === Inspector ===
    [Header("Cinemachine")]
    [Tooltip("CM_FollowCamera (CinemachineCamera 컴포넌트)")]
    [SerializeField] private CinemachineCamera cm;

    [Header("감도 (Sensitivity)")]
    [Tooltip("좌우 회전 감도 (px -> deg)")]
    [SerializeField] private float yawSpeed = 0.6f;
    [Tooltip("상하 회전 감도 (px -> deg)")]
    [SerializeField] private float pitchSpeed = 0.4f;
    [Tooltip("줌 감도 (scale)")]
    [SerializeField] private float zoomSpeed = 0.5f;

    [Header("제한 (Limits)")]
    [Tooltip("피벗-카메라 거리 범위")]
    [SerializeField] private float minDistance = 6f;
    [SerializeField] private float maxDistance = 16f;
    [Tooltip("피치 각도 제한(도)")]
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 90f;
    #endregion

    #region === Internals ===
    private CinemachineFollow _follow;   // FollowOffset 제어 대상
    private float _pitchDeg = 35f;       // 현재 피치(도)
    private bool _lastBlockOrbit;        // Debug용 상태 추적
    #endregion

    #region === Constants ===
    private const float Z_EPS = 0.3f;    // z>=0 방지(카메라 플립 방지용 버퍼)
    private const float WHEEL_SCALE = 0.1f;
    private const float PINCH_SCALE = 0.01f;
    #endregion

    #region === Input Actions ===
    private InputAction _lookDelta;      // <Pointer>/delta
    private InputAction _primary;        // */{PrimaryAction}
    private InputAction _secondary;      // */{SecondaryAction}
    private InputAction _scrollY;        // <Mouse>/scroll/y
    #endregion

    #region === Unity Lifecycle ===
    private void Awake()
    {
        if (!cm) cm = FindFirstObjectByType<CinemachineCamera>();
        _follow = cm ? cm.GetComponent<CinemachineFollow>() : null;

        if (!_follow)
        {
            Debug.LogError("[QuarterView] CinemachineFollow가 필요합니다. CM 카메라에 CinemachineFollow를 추가하세요.");
            enabled = false;
            return;
        }

        // 현재 FollowOffset에서 피치 초기화
        var o = _follow.FollowOffset;
        _pitchDeg = Mathf.Clamp(Rad2Deg(CurrentPitchRad(in o)), minPitch, maxPitch);

        // 입력 바인딩
        _lookDelta = new InputAction(type: InputActionType.PassThrough, binding: "<Pointer>/delta");
        _primary = new InputAction(type: InputActionType.Button, binding: "*/{PrimaryAction}");
        _secondary = new InputAction(type: InputActionType.Button, binding: "*/{SecondaryAction}");
        _scrollY = new InputAction(type: InputActionType.PassThrough, binding: "<Mouse>/scroll/y");
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        _lookDelta?.Enable();
        _primary?.Enable();
        _secondary?.Enable();
        _scrollY?.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        _lookDelta?.Disable();
        _primary?.Disable();
        _secondary?.Disable();
        _scrollY?.Disable();
    }

    private void OnDestroy()
    {
        _lookDelta?.Dispose();
        _primary?.Dispose();
        _secondary?.Dispose();
        _scrollY?.Dispose();
    }

    private void Update()
    {
        if (!_follow) return;

        HandlePointerInput();      // 마우스/터치 통합 입력 처리
        TrackBlockOrbitChange();   // 상태 추적 (디버그용)
        ClampZBehindTarget();      // z 안전 보정(플립 방지)
    }
    #endregion

    #region === Input ===
    /// <summary>마우스/터치 입력을 한 곳에서 분기 처리.</summary>
    private void HandlePointerInput()
    {
        HandleMouseInput();
        HandleTouchInput();
    }

    /// <summary>
    /// 마우스:
    /// - 좌/우 버튼 드래그: 회전 (단, BlockOrbit==true면 회전 차단)
    /// - 휠: 줌
    /// </summary>
    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        bool pressed = _primary.IsPressed() || _secondary.IsPressed();
        if (pressed && !EditModeController.BlockOrbit)
        {
            Vector2 delta = _lookDelta.ReadValue<Vector2>();
            RotateByDelta(delta);
        }

        float wheel = _scrollY.ReadValue<float>();
        if (Mathf.Abs(wheel) > 0.01f)
        {
            // 휠 위로 스크롤(양수)일 때 카메라가 가까워지도록 부호 반전
            ApplyZoom(-wheel * WHEEL_SCALE);
        }
    }

    /// <summary>
    /// 터치:
    /// - 1손가락 드래그: 회전 (BlockOrbit==true면 차단)
    /// - 2손가락 이상: 핀치 줌
    /// </summary>
    private void HandleTouchInput()
    {
        int count = TouchES.activeTouches.Count;
        if (count == 0) return;

        if (count == 1)
        {
            var t = TouchES.activeTouches[0];
            if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved && !EditModeController.BlockOrbit)
                RotateByDelta(t.delta);
        }
        else // 2손가락 이상 → 핀치 줌
        {
            var t0 = TouchES.activeTouches[0];
            var t1 = TouchES.activeTouches[1];

            Vector2 p0Prev = t0.screenPosition - t0.delta;
            Vector2 p1Prev = t1.screenPosition - t1.delta;

            float prevMag = (p0Prev - p1Prev).magnitude;
            float curMag = (t0.screenPosition - t1.screenPosition).magnitude;
            float pinch = curMag - prevMag;

            ApplyZoom(-pinch * PINCH_SCALE);
        }
    }

    /// <summary>BlockOrbit 플래그 변경 로그 추적(동작 변경 없음).</summary>
    private void TrackBlockOrbitChange()
    {
        if (EditModeController.BlockOrbit != _lastBlockOrbit)
            _lastBlockOrbit = EditModeController.BlockOrbit;
    }
    #endregion

    #region === Camera Ops ===
    /// <summary>드래그 델타를 야우/피치로 환산하여 적용.</summary>
    private void RotateByDelta(Vector2 delta)
    {
        // Yaw(수평 회전): 월드 Y축 기준
        transform.Rotate(Vector3.up, delta.x * yawSpeed, Space.World);

        // Pitch(수직 회전): 내부 각도 갱신 → FollowOffset 재계산
        _pitchDeg = Mathf.Clamp(_pitchDeg - delta.y * pitchSpeed, minPitch, maxPitch);
        ApplyPitchToOffset();
    }

    /// <summary>줌(거리만 변경, 현재 피치는 유지).</summary>
    private void ApplyZoom(float delta)
    {
        var o = _follow.FollowOffset;
        float curDist = CurrentDistance(in o);
        float newDist = Mathf.Clamp(curDist + delta * zoomSpeed, minDistance, maxDistance);

        float rad = Deg2Rad(_pitchDeg);
        o.y = Mathf.Sin(rad) * newDist;
        o.z = -Mathf.Cos(rad) * newDist;

        if (o.z > -Z_EPS) o.z = -Z_EPS; // 플립 방지
        _follow.FollowOffset = o;

        // 경계 보정 반영(수치 일관성 유지)
        _pitchDeg = Mathf.Clamp(Rad2Deg(CurrentPitchRad(in o)), minPitch, maxPitch);
    }

    /// <summary>현재 거리 유지하면서 피치만 FollowOffset에 반영.</summary>
    private void ApplyPitchToOffset()
    {
        var o = _follow.FollowOffset;
        float dist = CurrentDistance(in o);

        float rad = Deg2Rad(_pitchDeg);
        o.y = Mathf.Sin(rad) * dist;
        o.z = -Mathf.Cos(rad) * dist;

        if (o.z > -Z_EPS) o.z = -Z_EPS;
        _follow.FollowOffset = o;
    }

    /// <summary>항상 카메라가 피벗 뒤(-z)에 있도록 강제.</summary>
    private void ClampZBehindTarget()
    {
        var o = _follow.FollowOffset;
        if (o.z > -Z_EPS)
        {
            o.z = -Z_EPS;
            _follow.FollowOffset = o;
        }
    }
    #endregion

    #region === Math Helpers ===
    private static float CurrentDistance(in Vector3 followOffset)
        => Mathf.Sqrt(followOffset.y * followOffset.y + followOffset.z * followOffset.z);

    private static float CurrentPitchRad(in Vector3 followOffset)
        => Mathf.Atan2(followOffset.y, -followOffset.z);

    private static float Deg2Rad(float deg) => deg * Mathf.Deg2Rad;
    private static float Rad2Deg(float rad) => rad * Mathf.Rad2Deg;
    #endregion
}
