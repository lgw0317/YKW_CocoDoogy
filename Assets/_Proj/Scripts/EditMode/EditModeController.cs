using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchES = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// EditModeController (Core)
/// - 인스펙터 설정
/// - 공용 필드 (다른 partial에서 씀)
/// - 모드 on/off
/// - 저장/복원(Baseline)
/// </summary>
public partial class EditModeController : MonoBehaviour
{
    #region === Inspector ===

    [Header("Pick & Drag")]
    [SerializeField] private LayerMask draggableMask = ~0;

    [Header("Move Plane")]
    [SerializeField, Tooltip("드래그 시 Y를 처음 높이에 고정할지")]
    private bool lockYToInitial = true;
    [SerializeField, Tooltip("lockYToInitial = false 일 때 고정할 Y")]
    private float fixedY = 0f;

    [Header("Grid Snap")]
    [SerializeField] private bool snapToGrid = true;
    [SerializeField, Min(0.01f)] private float gridSize = 1f;
    [SerializeField, Tooltip("스냅 기준 원점")]
    private Vector3 gridOrigin = Vector3.zero;

    [Header("Undo")]
    [SerializeField, Tooltip("히스토리 최대 저장 개수 (-1: 무제한)")]
    private int undoMax = -1;

    [Header("Overlap")]
    [SerializeField, Tooltip("겹침 허용 오차")]
    private float overlapEpsilon = 0.0005f;

    [Header("UI (Toolbar/Undo)")]
    [SerializeField] private Button undoButton;
    [SerializeField] private ObjectActionToolbar actionToolbar;

    [Header("Edit Mode Entry (Long Press)")]
    [SerializeField, Tooltip("롱프레스 대상")]
    private Transform longPressTarget;
    [SerializeField, Tooltip("롱프레스 시간(초)")]
    private float longPressSeconds = 1.0f;
    [SerializeField, Tooltip("롱프레스 허용 이동량(px)")]
    private float longPressSlopPixels = 10f;

    [Header("Save/Back Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button backButton;

    [Header("Panels")]
    [SerializeField, Tooltip("편집 종료 확인 패널")]
    private GameObject exitConfirmPanel;
    [SerializeField] private Button exitYesButton;
    [SerializeField] private Button exitNoButton;
    [SerializeField, Tooltip("저장 완료 패널")]
    private GameObject savedInfoPanel;
    [SerializeField] private Button savedOkButton;

    [Header("Ground Limit")]
    [SerializeField, Tooltip("설치 가능한 바닥 레이어")]
    private LayerMask groundMask;
    [SerializeField, Tooltip("Ground 체크 위쪽 여유")]
    private float groundProbeUp = 3f;
    [SerializeField, Tooltip("Ground 체크 아래쪽 여유")]
    private float groundProbeDown = 6f;
    [SerializeField, Tooltip("무조건 Ground 위에서만 배치")]
    private bool requireGround = true;

    #endregion

    #region === Public State ===

    /// <summary>현재 편집모드 여부</summary>
    public bool IsEditMode { get; private set; }

    /// <summary>현재 선택/드래그 중인 오브젝트</summary>
    public Transform CurrentTarget { get; private set; }

    /// <summary>카메라(QuarterView)가 회전하면 안 되는 상태일 때 true</summary>
    public static bool BlockOrbit;

    #endregion

    #region === Shared State (다른 partial에서 씀) ===

    // 카메라
    private Camera cam;

    // 포인터 상태
    private bool pointerDown;
    private Vector2 pressScreenPos;
    private Transform pressedHitTarget;
    private bool isDragging;
    private bool movedDuringDrag;
    private bool currentPlacementValid = true;
    private bool startedOnDraggable;

    // 드래그용 평면
    private Plane movePlane;
    private float movePlaneY;
    private bool movePlaneReady;

    // Undo 히스토리 (오브젝트별)
    private struct Snap { public Vector3 pos; public Quaternion rot; }
    private readonly Dictionary<Transform, Stack<Snap>> history = new();

    // 롱프레스
    private bool longPressArmed;
    private float longPressTimer;
    private Vector2 longPressStartPos;

    // 저장/복원용 베이스라인
    private bool hasUnsavedChanges;
    private readonly List<ObjSnapshot> baseline = new();
    private readonly List<InventorySnapshot> invBaseline = new();

    // 인벤에서 막 꺼낸 오브젝트
    private Transform pendingFromInventory;

    #endregion

    #region === Snapshot Structs ===

    private struct ObjSnapshot
    {
        public Transform t;
        public Vector3 pos;
        public Quaternion rot;
        public bool activeSelf;
    }

    [Serializable]
    public struct InventorySnapshot
    {
        public int id;
        public int count;
    }

    #endregion

    private void Awake()
    {
        cam = Camera.main;
        if (!cam)
            Debug.LogWarning("[EditModeController] Main Camera를 찾지 못했습니다.");

        // 상단 버튼들 연결
        WireUndoButton();
        actionToolbar?.Hide();
        WireSaveButton();
        WireBackButton();
        WireExitPanels();
        WireSavedInfoPanel();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    private void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();

        BlockOrbit = false;
        isDragging = false;
        actionToolbar?.Hide();
    }

    private void Update()
    {
        // 입력/포인터 처리
        HandlePointerLifecycle();
        HandleLongPress();
        MaintainOrbitBlockFlag();
    }
}
