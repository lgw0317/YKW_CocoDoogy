using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchES = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// QuarterView
/// - 피벗(Tracking Target)을 기준으로 카메라 궤도 회전(Yaw/Pitch) + 거리(줌) 제어
/// - PC: 마우스 좌/우 드래그로 회전, 휠로 줌
/// - 모바일: 1손가락 드래그로 회전, 2손가락 핀치로 줌, (편집모드일 때) 2손가락 드래그로 팬
/// - 편집모드일 때 FollowTarget 아래에 임시 피벗을 만들어서 그걸 팬으로 움직인다
/// - 편집모드 빠질 때는 원래 FollowTarget 으로 되돌린다
/// </summary>
[DisallowMultipleComponent]
public class QuarterView : MonoBehaviour
{
    #region === Inspector ===

    [Header("Cinemachine")]
    [Tooltip("CinemachineCamera (예: CM_FollowCamera)")]
    [SerializeField] private CinemachineCamera cm;

    [Header("회전 감도")]
    [Tooltip("좌우(Yaw) 회전 감도 (px -> deg)")]
    [SerializeField] private float yawSpeed = 0.6f;
    [Tooltip("상하(Pitch) 회전 감도 (px -> deg)")]
    [SerializeField] private float pitchSpeed = 0.4f;

    [Header("줌 감도")]
    [Tooltip("줌 속도 계수")]
    [SerializeField] private float zoomSpeed = 0.5f;

    [Header("편집 모드 팬")]
    [Tooltip("픽셀 → 월드 변환 기본 스케일")]
    [SerializeField] private float panBaseScale = 0.0025f;
    [Tooltip("팬 속도 가중치")]
    [SerializeField] private float panSpeed = 1.0f;
    [Tooltip("마우스/손가락 이동과 반대로 화면이 움직이게 할지 여부")]
    [SerializeField] private bool invertEditPan = true;

    [Header("제한 (Limits)")]
    [Tooltip("피벗-카메라 최소 거리")]
    [SerializeField] private float minDistance = 6f;
    [Tooltip("피벗-카메라 최대 거리")]
    [SerializeField] private float maxDistance = 16f;
    [Tooltip("Pitch 최소 각도")]
    [SerializeField] private float minPitch = 5f;
    [Tooltip("Pitch 최대 각도")]
    [SerializeField] private float maxPitch = 90f;

    #endregion

    #region === Internals ===

    // CM
    private CinemachineFollow _follow;       // FollowOffset, FollowTarget 제어 대상
    private Transform _origFollowTarget;     // 편집모드 진입 전 FollowTarget
    private Transform _editPanPivot;         // 편집모드용 임시 피벗

    // 회전/줌 상태
    private float _pitchDeg = 35f;

    // 입력
    private InputAction _lookDelta;          // <Pointer>/delta
    private InputAction _primary;            // 좌클릭/탭
    private InputAction _secondary;          // 우클릭
    private InputAction _scrollY;            // 마우스 휠

    // 기타
    private Camera _cam;
    private bool _wasEditMode;
    private bool _lastBlockOrbit;            // 디버그용 추적

    // === UI-전역 오비트 차단 ===
    private static int _uiOrbitBlockCount = 0;
    public static bool IsUIOrbitBlocked => _uiOrbitBlockCount > 0;
    public static void PushUIOrbitBlock() { _uiOrbitBlockCount++; }
    public static void PopUIOrbitBlock() { _uiOrbitBlockCount = Mathf.Max(0, _uiOrbitBlockCount - 1); }

    #endregion

    #region === Const ===

    private const float Z_EPS = 0.3f;    // 카메라 플립 방지 z버퍼
    private const float WHEEL_SCALE = 0.1f;
    private const float PINCH_SCALE = 0.01f;

    #endregion


    #region === Unity Lifecycle ===

    private void Awake()
    {
        // CM 객체 가져오기
        if (!cm)
            cm = FindFirstObjectByType<CinemachineCamera>();

        _follow = cm ? cm.GetComponent<CinemachineFollow>() : null;
        _cam = Camera.main;

        if (!_follow)
        {
            Debug.LogError("[QuarterView] CinemachineFollow가 필요합니다. CM 카메라에 CinemachineFollow를 붙이세요.");
            enabled = false;
            return;
        }

        // 현재 FollowOffset으로부터 pitch 초기화
        var o = _follow.FollowOffset;
        _pitchDeg = Mathf.Clamp(Rad2Deg(CurrentPitchRad(in o)), minPitch, maxPitch);

        // 입력 Action 세팅
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

        // 1) 편집모드 상태 체크 → 피벗 교체
        SyncEditModePivot();

        // 2) 입력 처리
        HandlePointerInput();

        // 3) (디버그용) EditModeController.BlockOrbit 상태 추적
        TrackBlockOrbitChange();

        // 4) 카메라 z 플립 방지
        ClampZBehindTarget();
    }

    #endregion


    #region === 편집모드 피벗 교체 ===

    /// <summary>편집모드 진입/이탈을 감지해서 임시 피벗을 만든다/지운다.</summary>
    private void SyncEditModePivot()
    {
        bool isEdit = IsEditModeOn();

        if (isEdit == _wasEditMode)
            return; // 상태 변화 없음

        _wasEditMode = isEdit;

        if (isEdit) EnterEditPan();
        else ExitEditPan();
    }

    /// <summary>EditModeController가 있고 IsEditMode == true 인지 확인.</summary>
    private bool IsEditModeOn()
    {
        var emc = FindFirstObjectByType<EditModeController>();
        return emc != null && emc.IsEditMode;
    }

    /// <summary>편집모드 진입 시: FollowTarget 자식에 임시 피벗을 만들어서 그걸 Follow 로 바꾼다.</summary>
    private void EnterEditPan()
    {
        // 이미 만들어졌으면 무시
        if (_editPanPivot) return;

        _origFollowTarget = _follow.FollowTarget;
        if (_origFollowTarget == null)
        {
            Debug.LogWarning("[QuarterView] FollowTarget 이 비어 있어서 편집용 피벗을 만들 수 없습니다.");
            return;
        }

        var go = new GameObject("EditPanPivot (Runtime)");
        _editPanPivot = go.transform;
        _editPanPivot.SetParent(_origFollowTarget, worldPositionStays: false);
        _editPanPivot.localPosition = Vector3.zero;
        _editPanPivot.localRotation = Quaternion.identity;

        // 이제 카메라는 이 임시 피벗을 따라다닌다
        cm.Follow = _editPanPivot;
    }

    /// <summary>편집모드가 끝나면 임시 피벗을 파괴하고 FollowTarget 을 원복.</summary>
    private void ExitEditPan()
    {
        if (_editPanPivot)
            _editPanPivot.localPosition = Vector3.zero;

        if (_origFollowTarget)
            cm.Follow = _origFollowTarget;

        if (_editPanPivot)
            Destroy(_editPanPivot.gameObject);

        _editPanPivot = null;
        _origFollowTarget = null;
    }

    #endregion


    #region === 입력 처리 (PC + 모바일 공통) ===

    private void HandlePointerInput()
    {
        if (IsUIOrbitBlocked) return;
        HandleMouseInput();
        HandleTouchInput();
    }

    /// <summary>PC 입력 처리: 좌/우클릭 드래그, 휠 줌</summary>
    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        bool leftPressed = _primary.IsPressed();
        bool rightPressed = _secondary.IsPressed();

        // 1) 편집모드 + 우클릭 드래그 → 팬
        if (_editPanPivot && rightPressed && !IsPointerOverUI())
        {
            Vector2 delta = _lookDelta.ReadValue<Vector2>();
            ApplyPanFromScreenDelta(delta);
        }
        // 2) 일반 회전 (편집모드 아니거나, 편집모드인데 우클릭이 팬으로 안쓰일 때)
        else if ((leftPressed || rightPressed) &&
                 !EditModeController.BlockOrbit &&
                 !IsPointerOverUI())
        {
            Vector2 delta = _lookDelta.ReadValue<Vector2>();
            RotateByDelta(delta);
        }

        // 3) 휠 줌
        float wheel = _scrollY.ReadValue<float>();
        if (Mathf.Abs(wheel) > 0.01f)
        {
            ApplyZoom(-wheel * WHEEL_SCALE);
        }
    }

    /// <summary>모바일 터치 입력 처리</summary>
    private void HandleTouchInput()
    {
        int count = TouchES.activeTouches.Count;
        if (count == 0) return;

        // 1손가락 → 회전
        if (count == 1)
        {
            var t = TouchES.activeTouches[0];
            if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved &&
                !EditModeController.BlockOrbit &&
                !IsPointerOverUI())
            {
                RotateByDelta(t.delta);
            }
        }
        // 2손가락 이상 → 핀치 줌, (편집모드일 때) 팬
        else
        {
            var t0 = TouchES.activeTouches[0];
            var t1 = TouchES.activeTouches[1];

            // 핀치 줌 계산
            Vector2 p0Prev = t0.screenPosition - t0.delta;
            Vector2 p1Prev = t1.screenPosition - t1.delta;
            float prevMag = (p0Prev - p1Prev).magnitude;
            float curMag = (t0.screenPosition - t1.screenPosition).magnitude;
            float pinch = curMag - prevMag;
            ApplyZoom(-pinch * PINCH_SCALE);

            // 편집모드일 때만 평균 델타로 팬
            if (_editPanPivot && !IsPointerOverUI())
            {
                Vector2 avgDelta = 0.5f * (t0.delta + t1.delta);
                ApplyPanFromScreenDelta(avgDelta);
            }
        }
    }

    /// <summary>포인터가 UI 위에 있는지 검사 (마우스/첫번째 터치 기준)</summary>
    private static bool IsPointerOverUI()
    {
        if (!EventSystem.current) return false;

        Vector2 pos;
        if (Mouse.current != null)
            pos = Mouse.current.position.ReadValue();
        else if (TouchES.activeTouches.Count > 0)
            pos = TouchES.activeTouches[0].screenPosition;
        else
            return false;

        var data = new PointerEventData(EventSystem.current) { position = pos };
        var results = new System.Collections.Generic.List<RaycastResult>(8);
        EventSystem.current.RaycastAll(data, results);
        return results.Count > 0;
    }

    #endregion


    #region === 카메라 연산 ===

    /// <summary>화면 델타 → 편집 피벗 이동(XZ 평면)</summary>
    private void ApplyPanFromScreenDelta(Vector2 screenDelta)
    {
        if (!_editPanPivot) return;
        if (_cam == null) _cam = Camera.main;

        // 현재 거리로 스케일 보정
        var o = _follow.FollowOffset;
        float dist = Mathf.Max(1f, CurrentDistance(in o));
        float scale = dist * panBaseScale * panSpeed;

        // 화면 → 월드
        Vector3 right = _cam.transform.right; right.y = 0f; right.Normalize();
        Vector3 fwd = _cam.transform.forward; fwd.y = 0f; fwd.Normalize();

        float sign = invertEditPan ? -1f : 1f;
        Vector3 worldMove = (right * screenDelta.x + fwd * screenDelta.y) * (scale * sign);

        _editPanPivot.localPosition += worldMove;
    }

    /// <summary>마우스/터치 드래그로 회전</summary>
    private void RotateByDelta(Vector2 delta)
    {
        // 수평(Yaw): 월드 Y축 기준
        transform.Rotate(Vector3.up, delta.x * yawSpeed, Space.World);

        // 수직(Pitch): FollowOffset 값으로 계산
        _pitchDeg = Mathf.Clamp(_pitchDeg - delta.y * pitchSpeed, minPitch, maxPitch);
        ApplyPitchToOffset();
    }

    /// <summary>휠/핀치로 줌</summary>
    private void ApplyZoom(float delta)
    {
        var o = _follow.FollowOffset;

        float curDist = CurrentDistance(in o);
        float newDist = Mathf.Clamp(curDist + delta * zoomSpeed, minDistance, maxDistance);

        float rad = Deg2Rad(_pitchDeg);
        o.y = Mathf.Sin(rad) * newDist;
        o.z = -Mathf.Cos(rad) * newDist;

        // 플립 방지
        if (o.z > -Z_EPS)
            o.z = -Z_EPS;

        _follow.FollowOffset = o;

        // 경계 보정 후 pitch 다시 계산 (일관성 유지)
        _pitchDeg = Mathf.Clamp(Rad2Deg(CurrentPitchRad(in o)), minPitch, maxPitch);
    }

    /// <summary>현재 pitch 값으로 FollowOffset 적용</summary>
    private void ApplyPitchToOffset()
    {
        var o = _follow.FollowOffset;
        float dist = CurrentDistance(in o);

        float rad = Deg2Rad(_pitchDeg);
        o.y = Mathf.Sin(rad) * dist;
        o.z = -Mathf.Cos(rad) * dist;

        if (o.z > -Z_EPS)
            o.z = -Z_EPS;

        _follow.FollowOffset = o;
    }

    /// <summary>z가 너무 앞으로 넘어오는 것 방지</summary>
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


    #region === Debug Track ===

    private void TrackBlockOrbitChange()
    {
        if (EditModeController.BlockOrbit != _lastBlockOrbit)
            _lastBlockOrbit = EditModeController.BlockOrbit;
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
