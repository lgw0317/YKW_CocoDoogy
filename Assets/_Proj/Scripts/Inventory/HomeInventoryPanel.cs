using UnityEngine;
using UnityEngine.UI;

public class HomeInventoryPanel : MonoBehaviour
{
    [Header("DB & UI")]
    [SerializeField] private HomeDatabase homeDB;          // ← 인스펙터에 할당
    [SerializeField] private RectTransform content;        // Grid/Content
    [SerializeField] private GenericInvSlot slotPrefab;    // 공용 슬롯 프리팹

    public void OnEnable() => Rebuild();

    public void Rebuild()
    {
        if (!homeDB || !content || !slotPrefab) return;

        // 기존 비우기
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        // 슬롯 채우기
        foreach (var data in homeDB.homeList)
        {
            if (data == null) continue;
            var slot = Instantiate(slotPrefab, content);

            // 아이콘 로딩 (리소스 로더 바로 사용)
            var icon = data.GetIcon(new ResourcesLoader());
            slot.SetIcon(icon);
            slot.SetCountVisible(false);

            slot.SetOnClick(() =>
            {
                // 수량 소비 로직이 필요하면 나중에 추가 (지금은 미소비 스폰)
                var edit = FindFirstObjectByType<EditModeController>();
                if (!edit) return;
                edit.SpawnFromPlaceable(new HomePlaceable(data), PlaceableCategory.Home);
            });

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
}
