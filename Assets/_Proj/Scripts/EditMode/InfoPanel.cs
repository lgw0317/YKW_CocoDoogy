using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class InfoPanel : MonoBehaviour, IPointerClickHandler
{
    [Header("Wiring")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;

    [Header("Behavior")]
    [SerializeField] private bool closeOnBackgroundClick = true;
    [SerializeField] private bool closeOnEscape = true;
    [SerializeField, Tooltip("열린 직후 한 프레임 동안 배경 클릭 무시")]
    private bool ignoreBackgroundClickOnOpen = true;

    private int openedFrame = -1;
    private static InfoPanel _cached;

    private GameObject Root => root ? root : gameObject;
    public bool IsVisible => Root.activeSelf;

    private void Awake()
    {
        Hide();
        if (_cached == null) _cached = this;
    }

    private void Update()
    {
        if (closeOnEscape && IsVisible && Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Hide();
        }
    }

    public void Show(string title, string desc)
    {
        if (titleText) titleText.text = title ?? "";
        if (descText) descText.text = desc ?? "";
        openedFrame = Time.frameCount;
        Root.SetActive(true);
    }

    public void Show(ObjectMeta meta)
    {
        if (!meta)
        {
            Debug.LogWarning("[InfoPanel] meta가 null 입니다.");
            return;
        }
        Show(meta.DisplayName, meta.Description);
    }

    public void Hide()
    {
        if (Root) Root.SetActive(false);
    }

    public void Toggle(string title, string desc)
    {
        if (IsVisible) Hide();
        else Show(title, desc);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!closeOnBackgroundClick) return;
        if (ignoreBackgroundClickOnOpen && Time.frameCount == openedFrame) return;
        Hide();
    }

    public static InfoPanel FindInScene()
    {
        if (_cached && _cached.gameObject) return _cached;
#if UNITY_2022_2_OR_NEWER
        var panel = Object.FindFirstObjectByType<InfoPanel>(FindObjectsInactive.Include);
        if (!panel)
        {
            Debug.LogWarning("[InfoPanel] 씬에서 InfoPanel을 찾지 못했습니다.");
            return null;
        }
        _cached = panel;
        return _cached;
#else
        var all = Resources.FindObjectsOfTypeAll<InfoPanel>();
        foreach (var p in all)
        {
            if (p && p.gameObject.scene.IsValid())
            {
                _cached = p;
                return _cached;
            }
        }
        Debug.LogWarning("[InfoPanel] 씬에서 InfoPanel을 찾지 못했습니다.");
        return null;
#endif
    }
}
