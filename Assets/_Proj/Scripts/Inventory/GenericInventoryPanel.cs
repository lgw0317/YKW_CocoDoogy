using UnityEngine;
using UnityEngine.UI;

public enum InventoryCategory
{
    Home,
    Background,
    Animal,
    Deco
}

/// <summary>
/// 공용 인벤토리 목록 패널
/// - 한 개의 ScrollView 안에서 여러 카테고리의 아이템을 표시.
/// - 탭이 바뀌면 Rebuild(category)로 다시 그림.
/// </summary>
public class GenericInventoryPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform content;    // ScrollView/Viewport/Content
    [SerializeField] private GenericInvSlot slotPrefab;

    [Header("DB")]
    [SerializeField] private HomeDatabase homeDB;
    [SerializeField] private BackgroundDatabase backgroundDB;
    [SerializeField] private AnimalDatabase animalDB;
    [SerializeField] private DecoDatabase decoDB;

    public void Rebuild(InventoryCategory category)
    {
        if (!content || !slotPrefab) return;

        // 기존 슬롯 제거
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        switch (category)
        {
            case InventoryCategory.Home:
                if (homeDB) foreach (var d in homeDB.homeList) MakeSlot(d?.GetIcon(new ResourcesLoader()), d?.home_name);
                break;

            case InventoryCategory.Background:
                if (backgroundDB) foreach (var d in backgroundDB.bgList) MakeSlot(d?.GetIcon(new ResourcesLoader()), d?.bg_name);
                break;

            case InventoryCategory.Animal:
                if (animalDB) foreach (var d in animalDB.animalList) MakeSlot(d?.GetIcon(new ResourcesLoader()), d?.animal_name);
                break;

            case InventoryCategory.Deco:
                if (decoDB)
                {
                    foreach (var d in decoDB.decoList)
                    {
                        var slot = Instantiate(slotPrefab, content);
                        slot.SetIcon(d.GetIcon(new ResourcesLoader()));
                        slot.SetCountVisible(true, $"x{DecoInventoryRuntime.I.Count(d.deco_id)}");
                        slot.SetOnClick(() =>
                        {
                            if (DecoInventoryRuntime.I.TryConsume(d.deco_id, 1))
                            {
                                FindFirstObjectByType<EditModeController>()?.SpawnFromDecoData(d);
                            }
                        });
                    }
                }
                break;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private void MakeSlot(Sprite icon, string name)
    {
        var slot = Instantiate(slotPrefab, content);
        slot.SetIcon(icon);
        slot.SetCountVisible(false);
        slot.SetOnClick(() =>
        {
            Debug.Log($"Clicked item: {name}");
        });
    }
}
