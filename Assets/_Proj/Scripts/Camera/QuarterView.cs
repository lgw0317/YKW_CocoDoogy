using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchES = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// QuarterView
/// - 피벗(Tracking Target) 기준 궤도 회전(Yaw/Pitch) + 거리(줌)
/// - PC: 좌/우 드래그 회전, 휠 줌 / (편집모드) 우클릭 드래그 팬
/// - 모바일: 1손가락 회전, 2손가락 핀치 줌, (편집모드) 2손가락 드래그 팬
/// - 편집모드에서 FollowTarget의 자식으로 임시 피벗을 생성해 팬 전용으로 사용
/// - 편집모드 해제 시 FollowTarget을 원래대로 복구
/// - UI가 오비트를 막아야 할 때는 PushUIOrbitBlock/PopUIOrbitBlock 사용
/// </summary>
[DisallowMultipleComponent]
public class QuarterView : MonoBehaviour
{
    #region ===== Inspector =====

    [Header("Cinemachine")]
    [Tooltip("CinemachineCamera (예: CM_FollowCamera)")]
    [SerializeField] private CinemachineCamera cm;

    [Header("회전 감도")]
    [SerializeField, Tooltip("좌우(Yaw) 회전 감도 (px → deg)")]
    private float yawSpeed = 0.6f;
    [SerializeField, Tooltip("상하(Pitch) 회전 감도 (px → deg)")]
    private float pitchSpeed = 0.4f;

    [Header("줌 감도")]
    [SerializeField, Tooltip("줌 속도 계수")]
    private float zoomSpeed = 0.5f;

    [Header("편집모드 팬")]
    [SerializeField, Tooltip("픽셀 → 월드 기본 스케일")]
    private float panBaseScale = 0.0025f;
    [SerializeField, Tooltip("팬 속도 가중치")]
    private float panSpeed = 1.0f;
    [SerializeField, Tooltip("드래그 방향 반전")]
    private bool invertEditPan = true;

    [Header("제한 (Limits)")]
    [SerializeField, Tooltip("피벗-카메라 최소 거리")]
    private float minDistance = 6f;
    [SerializeField, Tooltip("피벗-카메라 최대 거리")]
    private float maxDistance = 16f;
    [SerializeField, Tooltip("Pitch 최소 각도")]
    private float minPitch = 5f;
    [SerializeField, Tooltip("Pitch 최대 각도")]
    private float maxPitch = 90f;

    [Header("옵션")]
    [SerializeField, Tooltip("지정하지 않으면 최초 1회 자동 검색")]
    private EditModeController editController;

    [Header("편집모드 팬 제한")]
    [SerializeField] private bool limitEditPan = true;                 // 제한 사용
    [SerializeField, Min(0f)] private float panLimitRadius = 12f;      // 원형 반경(m)
    [SerializeField] private bool useRectLimit = false;                // 직사각형 제한 사용
    [SerializeField] private Vector2 panHalfSize = new Vector2(12f, 12f); // AABB half size(m)

    #endregion

    #region ===== Runtime State =====

    private CinemachineFollow _follow;   // FollowOffset/Target 제어
    private Transform _origFollowTarget; // 편집모드 전 FollowTarget
    private Transform _editPanPivot;     // 편집모드 임시 피벗

    private float _pitchDeg = 35f;

    private Camera _cam;
    private bool _wasEditMode;
    private bool _lastBlockOrbit;
    private bool _hasEventSystem;

    // 입력 액션
    private InputAction _lookDelta;      // <Pointer>/delta
    private InputAction _primary;        // */{PrimaryAction}
    private InputAction _secondary;      // */{SecondaryAction}
    private InputAction _scrollY;        // <Mouse>/scroll/y

    // 포인터 ID (UI 히트 테스트용)
    private int _mousePointerId = PointerInputModule.kMouseLeftId; // -1 (참고용)
    private int _touchPointerId = 0; // 첫 번째 터치

    // 전역 오비트 차단 카운터 (UI에서 호출)
    private static int _uiOrbitBlockCount = 0;
    public static bool IsUIOrbitBlocked => _uiOrbitBlockCount > 0;
    public static void PushUIOrbitBlock() { _uiOrbitBlockCount++; }
    public static void PopUIOrbitBlock() { _uiOrbitBlockCount = Mathf.Max(0, _uiOrbitBlockCount - 1); }

    #endregion

    #region ===== Constants =====

    private const float Z_EPS = 0.3f;     // 카메라 플립 방지 z 버퍼
    private const float WHEEL_SCALE = 0.1f;
    private const float PINCH_SCALE = 0.01f;

    #endregion

    #region ===== Unity Lifecycle =====

    private void Reset()
    {
        if (!cm) cm = GetComponentInChildren<CinemachineCamera>();
    }

    private void Awake()
    {
        if (!cm)
            cm = FindFirstObjectByType<CinemachineCamera>(FindObjectsInactive.Exclude);

        _follow = cm ? cm.GetComponent<CinemachineFollow>() : null;
        _cam = Camera.main;

        if (!_follow)
        {
            Debug.LogError("[QuarterView] CinemachineFollow가 필요합니다. CM 카메라에 CinemachineFollow를 추가하세요.");
            enabled = false;
            return;
        }

        // pitch 초기화
        var o = _follow.FollowOffset;
        _pitchDeg = Mathf.Clamp(Rad2Deg(CurrentPitchRad(in o)), minPitch, maxPitch);

        // 입력 액션
        _lookDelta = new InputAction(type: InputActionType.PassThrough, binding: "<Pointer>/delta");
        _primary = new InputAction(type: InputActionType.Button, binding: "*/{PrimaryAction}");
        _secondary = new InputAction(type: InputActionType.Button, binding: "*/{SecondaryAction}");
        _scrollY = new InputAction(type: InputActionType.PassThrough, binding: "<Mouse>/scroll/y");

        // EventSystem 캐시(없으면 UI 차단 로직 생략)
        _hasEventSystem = EventSystem.current != null;

        // EditModeController 캐시(없으면 한 번만 시도)
        if (!editController)
            editController = FindFirstObjectByType<EditModeController>(FindObjectsInactive.Exclude);
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
        _lookDelta?.Disable();
        _primary?.Disable();
        _secondary?.Disable();
        _scrollY?.Disable();
        // EnhancedTouchSupport.Disable(); // 전역 기능: 여기서 끄지 않음
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

        SyncEditModePivot();
        HandlePointerInput();
        TrackBlockOrbitChange();
        ClampZBehindTarget();
    }

    #endregion

    #region ===== Edit Mode Pivot (생성/복구) =====

    private bool IsEditModeOn()
    {
        if (!editController)
            editController = FindFirstObjectByType<EditModeController>(FindObjectsInactive.Exclude);
        return editController && editController.IsEditMode;
    }

    private void SyncEditModePivot()
    {
        bool isEdit = IsEditModeOn();
        if (isEdit == _wasEditMode) return;

        _wasEditMode = isEdit;
        if (isEdit) EnterEditPan();
        else ExitEditPan();
    }

    private void EnterEditPan()
    {
        if (_editPanPivot) return;

        _origFollowTarget = _follow.FollowTarget;
        if (!_origFollowTarget)
        {
            Debug.LogWarning("[QuarterView] FollowTarget이 비어 있어 편집용 피벗을 만들 수 없습니다.");
            return;
        }

        var go = new GameObject("EditPanPivot (Runtime)");
        _editPanPivot = go.transform;
        _editPanPivot.SetParent(_origFollowTarget, worldPositionStays: false);
        _editPanPivot.localPosition = Vector3.zero;
        _editPanPivot.localRotation = Quaternion.identity;

        cm.Follow = _editPanPivot;
    }

    private void ExitEditPan()
    {
        if (_editPanPivot) _editPanPivot.localPosition = Vector3.zero;
        if (_origFollowTarget) cm.Follow = _origFollowTarget;

        if (_editPanPivot)
            Destroy(_editPanPivot.gameObject);

        _editPanPivot = null;
        _origFollowTarget = null;
    }

    #endregion

    #region ===== Input Handling (PC & Mobile) =====

    private void HandlePointerInput()
    {
        if (IsUIOrbitBlocked) return;

        HandleMouseInput();
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        bool leftPressed = _primary.IsPressed();
        bool rightPressed = _secondary.IsPressed();

        // 팬: 편집모드 + 우클릭 드래그
        if (_editPanPivot && rightPressed && !IsPointerOverUI())
        {
            Vector2 delta = _lookDelta.ReadValue<Vector2>();
            ApplyPanFromScreenDelta(delta);
        }
        // 회전
        else if ((leftPressed || rightPressed) &&
                 !EditModeController.BlockOrbit &&
                 !IsPointerOverUI())
        {
            Vector2 delta = _lookDelta.ReadValue<Vector2>();
            RotateByDelta(delta);
        }

        // 줌
        float wheel = _scrollY.ReadValue<float>();
        if (Mathf.Abs(wheel) > 0.01f)
            ApplyZoom(-wheel * WHEEL_SCALE);
    }

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
        // 2손가락 이상 → 핀치 줌 + (편집모드) 팬
        else
        {
            var t0 = TouchES.activeTouches[0];
            var t1 = TouchES.activeTouches[1];

            Vector2 p0Prev = t0.screenPosition - t0.delta;
            Vector2 p1Prev = t1.screenPosition - t1.delta;
            float prevMag = (p0Prev - p1Prev).magnitude;
            float curMag = (t0.screenPosition - t1.screenPosition).magnitude;
            float pinch = curMag - prevMag;

            ApplyZoom(-pinch * PINCH_SCALE);

            if (_editPanPivot && !IsPointerOverUI())
            {
                Vector2 avgDelta = 0.5f * (t0.delta + t1.delta);
                ApplyPanFromScreenDelta(avgDelta);
            }
        }
    }

    /// <summary>
    /// 현재 포인터가 UI 위인지 판정.
    /// - 마우스: EventSystem.IsPointerOverGameObject()
    /// - 터치:   첫 터치 id 사용
    /// - EventSystem 없으면 false
    /// </summary>
    private bool IsPointerOverUI()
    {
        if (!_hasEventSystem || EventSystem.current == null) return false;

        if (Mouse.current != null)
            return EventSystem.current.IsPointerOverGameObject();

        if (TouchES.activeTouches.Count > 0)
            return EventSystem.current.IsPointerOverGameObject(_touchPointerId);

        return false;
    }

    #endregion

    #region ===== Camera Ops (회전/줌/팬/클램프) =====

    private void ApplyPanFromScreenDelta(Vector2 screenDelta)
    {
        if (!_editPanPivot) return;
        if (!_cam) _cam = Camera.main;

        // 거리 기반 스케일링
        var o = _follow.FollowOffset;
        float dist = Mathf.Max(1f, CurrentDistance(in o));
        float scale = dist * panBaseScale * panSpeed;

        // 화면 기준 벡터를 XZ 평면으로 투영
        Vector3 right = _cam.transform.right; right.y = 0f; right.Normalize();
        Vector3 fwd = _cam.transform.forward; fwd.y = 0f; fwd.Normalize();

        float sign = invertEditPan ? -1f : 1f;

        // 월드 이동량 계산 후 월드 위치로 적용 (localPosition 사용 금지)
        Vector3 worldMove = (right * screenDelta.x + fwd * screenDelta.y) * (scale * sign);
        Vector3 targetPos = _editPanPivot.position + worldMove;
        _editPanPivot.position = ClampEditPanWorld(targetPos);
    }

    private void RotateByDelta(Vector2 delta)
    {
        // 수평(Yaw)
        transform.Rotate(Vector3.up, delta.x * yawSpeed, Space.World);

        // 수직(Pitch)
        _pitchDeg = Mathf.Clamp(_pitchDeg - delta.y * pitchSpeed, minPitch, maxPitch);
        ApplyPitchToOffset();
    }

    private void ApplyZoom(float delta)
    {
        var o = _follow.FollowOffset;

        float curDist = CurrentDistance(in o);
        float newDist = Mathf.Clamp(curDist + delta * zoomSpeed, minDistance, maxDistance);

        float rad = Deg2Rad(_pitchDeg);
        o.y = Mathf.Sin(rad) * newDist;
        o.z = -Mathf.Cos(rad) * newDist;

        if (o.z > -Z_EPS) o.z = -Z_EPS;

        _follow.FollowOffset = o;

        // 보정 후 pitch 재계산
        _pitchDeg = Mathf.Clamp(Rad2Deg(CurrentPitchRad(in o)), minPitch, maxPitch);
    }

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

    private void ClampZBehindTarget()
    {
        var o = _follow.FollowOffset;
        if (o.z > -Z_EPS)
        {
            o.z = -Z_EPS;
            _follow.FollowOffset = o;
        }
    }

    private Vector3 ClampEditPanWorld(Vector3 desiredWorldPos)
    {
        if (!limitEditPan) return desiredWorldPos;

        // 제한 기준 중심: 원래 FollowTarget (없으면 현재 피벗)
        Vector3 center =
            _origFollowTarget ? _origFollowTarget.position :
            (_editPanPivot ? _editPanPivot.position : Vector3.zero);

        float y = desiredWorldPos.y; // 높이는 유지
        Vector3 offset = desiredWorldPos - center;
        offset.y = 0f;

        if (useRectLimit)
        {
            // 직사각형(AABB) 제한
            float clampedX = Mathf.Clamp(offset.x, -panHalfSize.x, panHalfSize.x);
            float clampedZ = Mathf.Clamp(offset.z, -panHalfSize.y, panHalfSize.y);
            return new Vector3(center.x + clampedX, y, center.z + clampedZ);
        }
        else
        {
            // 원형(반경) 제한
            float mag = offset.magnitude;
            if (mag <= panLimitRadius) return desiredWorldPos;

            Vector3 clamped = offset.normalized * panLimitRadius;
            return new Vector3(center.x + clamped.x, y, center.z + clamped.z);
        }
    }

    #endregion

    #region ===== Debug/Tracking =====

    private void TrackBlockOrbitChange()
    {
        if (EditModeController.BlockOrbit != _lastBlockOrbit)
            _lastBlockOrbit = EditModeController.BlockOrbit;
    }

    #endregion

    #region ===== Math Helpers =====

    private static float CurrentDistance(in Vector3 followOffset)
        => Mathf.Sqrt(followOffset.y * followOffset.y + followOffset.z * followOffset.z);

    private static float CurrentPitchRad(in Vector3 followOffset)
        => Mathf.Atan2(followOffset.y, -followOffset.z);

    private static float Deg2Rad(float deg) => deg * Mathf.Deg2Rad;
    private static float Rad2Deg(float rad) => rad * Mathf.Rad2Deg;

    #endregion
}
