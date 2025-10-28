using UnityEngine;

[DisallowMultipleComponent]
public class ObjectMeta : MonoBehaviour
{
    [Header("Meta")]
    [SerializeField] private string displayName = "이름 없음";
    [TextArea, SerializeField] private string description = "설명이 없습니다.";

    public string DisplayName => displayName;
    public string Description => description;

    public void ShowInfo()
    {
        var panel = InfoPanel.FindInScene();
        if (!panel)
        {
            Debug.LogWarning("[ObjectMeta] InfoPanel을 씬에서 찾지 못했습니다.");
            return;
        }
        panel.Show(displayName, description);
    }
}
