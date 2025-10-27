using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class InfoPanel : MonoBehaviour, IPointerClickHandler
{
    [Header("Wiring")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;

    private static InfoPanel _cached;

    private GameObject Root => root ? root : gameObject;
    public bool IsVisible => Root.activeSelf;

    private void Awake()
    {
        Hide();
        if (_cached == null) _cached = this;
    }

    public void Show(string title, string desc)
    {
        if (titleText) titleText.text = title ?? "";
        if (descText) descText.text = desc ?? "";
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

    // 배경 클릭 시 닫기 (옵션 제거: 항상 닫음)
    public void OnPointerClick(PointerEventData eventData)
    {
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
