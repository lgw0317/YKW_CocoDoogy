using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GenericInvSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;   // 없으면 자동 무시
    [SerializeField] private Button clickArea;

    private Action _onClick;

    private void Awake()
    {
        if (clickArea)
        {
            clickArea.onClick.RemoveAllListeners();
            clickArea.onClick.AddListener(() => _onClick?.Invoke());
        }
    }

    public void SetIcon(Sprite s)
    {
        if (icon) icon.sprite = s;
    }

    public void SetCountVisible(bool visible, string text = "")
    {
        if (!countText) return;
        countText.gameObject.SetActive(visible);
        if (visible) countText.text = text ?? "";
    }

    public void SetOnClick(Action onClick) => _onClick = onClick;
}
