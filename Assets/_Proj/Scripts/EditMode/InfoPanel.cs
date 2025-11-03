using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 선택한 오브젝트 설명을 띄우는 단순 패널
/// - Show(title, desc) : 무조건 열고 내용 세팅
/// - Toggle(title, desc)
///   - 이미 같은 title 로 열려 있으면 닫기
///   - 다른 title 이 열려 있으면 그거 닫고 새로 열기
/// - 패널 바깥(배경) 클릭하면 닫기
/// </summary>
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

    // 현재 어떤 오브젝트 설명을 보여주는지
    private string currentTitle;

    private void Awake()
    {
        Hide();
        if (_cached == null) _cached = this;
    }

    /// <summary>무조건 열기</summary>
    public void Show(string title, string desc)
    {
        currentTitle = title;

        if (titleText) titleText.text = title ?? string.Empty;
        if (descText) descText.text = desc ?? string.Empty;

        Root.SetActive(true);
    }

    /// <summary>무조건 닫기</summary>
    public void Hide()
    {
        if (Root) Root.SetActive(false);
        currentTitle = null;
    }

    /// <summary>
    /// 같은 타이틀이면 닫고,
    /// 다른 타이틀이면 그걸로 다시 연다.
    /// </summary>
    public void Toggle(string title, string desc)
    {
        // 1) 이미 열려 있고 같은 대상 → 닫기
        if (IsVisible && title == currentTitle)
        {
            Hide();
            return;
        }

        // 2) 그 외는 새로 열기
        Show(title, desc);
    }

    /// <summary>배경 눌렀을 때 닫기</summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Hide();
    }

    /// <summary>씬 안에서 InfoPanel 하나 찾아오기 (싱글턴 느낌)</summary>
    public static InfoPanel FindInScene()
    {
        if (_cached && _cached.gameObject) return _cached;

#if UNITY_2022_2_OR_NEWER
        var panel = Object.FindFirstObjectByType<InfoPanel>(FindObjectsInactive.Include);
        if (!panel)
        {
            Debug.LogWarning("[InfoPanel] 씬에서 InfoPanel 을 찾지 못했습니다.");
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
        Debug.LogWarning("[InfoPanel] 씬에서 InfoPanel 을 찾지 못했습니다.");
        return null;
#endif
    }
}
