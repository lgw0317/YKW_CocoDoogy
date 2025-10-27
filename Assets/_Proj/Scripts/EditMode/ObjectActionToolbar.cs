using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ObjectActionToolbar : MonoBehaviour
{
    #region === Types ===
    public enum AnchorMode { Transform, BoundsTop }
    #endregion

    #region === Inspector ===
    [Header("Wiring")]
    [SerializeField] private Canvas canvas;      // 툴바가 속한 캔버스
    [SerializeField] private Button btnInfo;
    [SerializeField] private Button btnRotate;
    [SerializeField] private Button btnInventory;
    [SerializeField] private Button btnOk;
    [SerializeField] private Button btnCancel;

    [Header("Anchor")]
    [SerializeField] private AnchorMode anchorMode = AnchorMode.BoundsTop;
    [SerializeField, Tooltip("BoundsTop 기준일 때 윗면에서 더 띄울 높이(m)")]
    private float extraHeight = 0.15f;
    [SerializeField, Tooltip("Transform 기준일 때 월드 오프셋(m)")]
    private Vector3 worldOffset = new(0f, 1.2f, 0f);

    [Header("Screen")]
    [SerializeField, Tooltip("화면에서의 추가 오프셋(px)")]
    private Vector2 screenOffset = new(0f, 16f);
    [SerializeField, Tooltip("화면 밖으로 나가지 않도록 클램프")]
    private bool clampToScreen = true;
    [SerializeField, Tooltip("클램프 패딩(px)")]
    private Vector2 clampPadding = new(8f, 8f);
    [SerializeField, Tooltip("Overlay에서 SafeArea 고려(권장)")]
    private bool respectSafeArea = true;

    [Header("Follow")]
    [SerializeField, Tooltip("부드럽게 따라오기(0=즉시, 10=매우부드러움)")]
    private float followLerp = 0f;

    [Header("Visibility")]
    [SerializeField, Tooltip("대상이 카메라 뒤(z<0)이면 자동 숨김")]
    private bool autoHideWhenBehindCamera = true;
    #endregion

    #region === State ===
    private Transform target;        // 따라다닐 대상
    private Camera cam;              // 월드 카메라
    private RectTransform rect;      // 이 툴바의 RectTransform
    private Vector2 currentAnchored; // 스무딩 중간값
    private bool visibleByCamera = true;
    #endregion

    #region === Unity Lifecycle ===
    private void Awake()
    {
        rect = transform as RectTransform;
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false); // 기본 숨김
    }

    private void LateUpdate()
    {
        if (!CanUpdatePosition()) return;

        // 카메라 뒤면 숨김 처리
        visibleByCamera = IsTargetInFrontOfCamera();
        if (autoHideWhenBehindCamera && !visibleByCamera)
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
            return;
        }
        else if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Vector2 screenPos = CalcScreenPos();
        SetAnchoredPosition(screenPos);
    }
    #endregion

    #region === Public API ===
    /// <summary>툴바 표시 및 버튼 콜백 설정.</summary>
    public void Show(
        Transform target,
        Camera worldCamera,
        Action onInfo = null,
        Action onRotate = null,
        Action onInventory = null,
        Action onOk = null,
        Action onCancel = null)
    {
        this.target = target;
        cam = worldCamera ? worldCamera : Camera.main;

        Wire(btnInfo, onInfo);
        Wire(btnRotate, onRotate);
        Wire(btnInventory, onInventory);
        Wire(btnOk, onOk);
        Wire(btnCancel, onCancel);

        // 카메라 뒤면 우선 숨김
        visibleByCamera = IsTargetInFrontOfCamera();
        gameObject.SetActive(visibleByCamera);

        // 초기 위치 즉시 반영
        Vector2 pos = CalcScreenPos();
        SetAnchoredPositionImmediate(pos);
    }

    /// <summary>툴바 숨김 및 버튼 콜백 해제.</summary>
    public void Hide()
    {
        if (!this) return;

        gameObject.SetActive(false);
        target = null;
        cam = null;

        Unwire(btnInfo);
        Unwire(btnRotate);
        Unwire(btnInventory);
        Unwire(btnOk);
        Unwire(btnCancel);
    }
    #endregion

    #region === Positioning ===
    /// <summary>대상 월드포지션 → 스크린/캔버스 좌표 변환.</summary>
    private Vector2 CalcScreenPos()
    {
        Vector3 worldPos = GetAnchorWorldPosition();
        if (!cam) cam = Camera.main;

        // 월드 → 스크린(px)
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        screen += screenOffset;

        // 화면 클램프
        if (clampToScreen)
        {
            Vector2 min = clampPadding;
            Vector2 max = new Vector2(Screen.width, Screen.height) - clampPadding;

            // SafeArea 고려(Overlay 전용)
            if (respectSafeArea && canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Rect sa = Screen.safeArea;
                min = new Vector2(Mathf.Max(min.x, sa.xMin), Mathf.Max(min.y, sa.yMin));
                max = new Vector2(Mathf.Min(max.x, sa.xMax), Mathf.Min(max.y, sa.yMax));
            }

            screen.x = Mathf.Clamp(screen.x, min.x, max.x);
            screen.y = Mathf.Clamp(screen.y, min.y, max.y);
        }

        // Canvas 좌표계로 변환
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Overlay: 스크린 == 앵커드
            return screen;
        }
        else
        {
            // ScreenSpace-Camera / WorldSpace
            var canvasRect = canvas ? canvas.transform as RectTransform : null;
            if (canvasRect &&
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, canvas ? canvas.worldCamera : cam, out var local))
            {
                return local;
            }
            // 실패 시 직전 값 유지
            return currentAnchored;
        }
    }

    /// <summary>앵커 모드에 따른 월드 기준점 계산.</summary>
    private Vector3 GetAnchorWorldPosition()
    {
        if (!target) return Vector3.zero;

        if (anchorMode == AnchorMode.BoundsTop)
        {
            if (TryGetWorldBounds(target, out Bounds b))
                return new Vector3(b.center.x, b.max.y + extraHeight, b.center.z);

            // 렌더러/콜라이더가 없으면 Transform 기준 보정
            return target.position + new Vector3(0f, Mathf.Max(extraHeight, 0f), 0f);
        }
        // Transform
        return target.position + worldOffset;
    }

    /// <summary>위치 즉시 적용(스무딩 없음).</summary>
    private void SetAnchoredPositionImmediate(Vector2 p)
    {
        currentAnchored = p;
        rect.anchoredPosition = p;
    }

    /// <summary>위치 적용(스무딩 옵션).</summary>
    private void SetAnchoredPosition(Vector2 p)
    {
        if (followLerp <= 0f)
        {
            SetAnchoredPositionImmediate(p);
            return;
        }

        float t = 1f - Mathf.Exp(-followLerp * Time.unscaledDeltaTime); // 지수 감쇠
        currentAnchored = Vector2.Lerp(currentAnchored, p, t);
        rect.anchoredPosition = currentAnchored;
    }
    #endregion

    #region === Helpers ===
    private void Wire(Button b, Action cb)
    {
        if (!b) return;
        b.onClick.RemoveAllListeners();
        if (cb != null) b.onClick.AddListener(() => cb());
        b.gameObject.SetActive(cb != null);

        // 비활성 CanvasGroup이 감싸고 있을 수 있으니 인터랙션 보장
        var cg = b.GetComponentInParent<CanvasGroup>();
        if (cg) cg.interactable = true;
    }

    private static void Unwire(Button b)
    {
        if (!b) return;
        b.onClick.RemoveAllListeners();
        // Hide() 후에는 표시 여부는 외부 로직 또는 Show()에서 재결정
    }

    private bool CanUpdatePosition()
    {
        return rect && canvas && target && cam;
    }

    private bool IsTargetInFrontOfCamera()
    {
        if (!target || !cam) return true;
        Vector3 to = cam.worldToCameraMatrix.MultiplyPoint(target.position);
        // 카메라 좌표계에서 z < 0 은 카메라 앞 (Unity의 Clip Space 이전 단계),
        // worldToCameraMatrix 기준으로는 일반적으로 '양수면 뒤'인 파이프라인도 있으므로 Viewport 기반으로 한 번 더 보정
        Vector3 vp = cam.WorldToViewportPoint(target.position);
        return vp.z > 0f; // 뒷면이면 음수
    }

    /// <summary>대상 트리의 렌더러/콜라이더 바운즈 합산.</summary>
    private static bool TryGetWorldBounds(Transform t, out Bounds bounds)
    {
        bounds = default;
        bool has = false;

        // 1) Renderer 우선(시각 기준)
        var rs = t.GetComponentsInChildren<Renderer>(includeInactive: false);
        for (int i = 0; i < rs.Length; i++)
        {
            var r = rs[i];
            if (!r) continue;
            if (!has) { bounds = r.bounds; has = true; }
            else bounds.Encapsulate(r.bounds);
        }

        // 2) 없으면 Collider
        if (!has)
        {
            var cs = t.GetComponentsInChildren<Collider>(includeInactive: false);
            for (int i = 0; i < cs.Length; i++)
            {
                var c = cs[i];
                if (!c) continue;
                if (!has) { bounds = c.bounds; has = true; }
                else bounds.Encapsulate(c.bounds);
            }
        }

        return has;
    }
    #endregion
}
