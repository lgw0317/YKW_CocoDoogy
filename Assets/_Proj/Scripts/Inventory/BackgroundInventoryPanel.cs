using UnityEngine;
using UnityEngine.UI;

public class BackgroundInventoryPanel : MonoBehaviour
{
    [Header("DB & UI")]
    [SerializeField] private BackgroundDatabase bgDB;      // ← 인스펙터에 할당
    [SerializeField] private RectTransform content;
    [SerializeField] private GenericInvSlot slotPrefab;

    public void OnEnable() => Rebuild();

    public void Rebuild()
    {
        if (!bgDB || !content || !slotPrefab) return;

        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        foreach (var data in bgDB.bgList)
        {
            if (data == null) continue;
            var slot = Instantiate(slotPrefab, content);

            var icon = data.GetIcon(new ResourcesLoader());
            slot.SetIcon(icon);
            slot.SetCountVisible(false);

            slot.SetOnClick(() =>
            {
                Debug.Log($"[BackgroundInventory] 클릭: {data.bg_name} ({data.bg_id})");
                // TODO: RenderSettings.skybox = data.GetMaterial(new ResourcesLoader());
            });
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
}
