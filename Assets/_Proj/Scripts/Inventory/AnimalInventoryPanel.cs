using UnityEngine;
using UnityEngine.UI;

public class AnimalInventoryPanel : MonoBehaviour
{
    [Header("DB & UI")]
    [SerializeField] private AnimalDatabase animalDB;      // ← 인스펙터에 할당
    [SerializeField] private RectTransform content;
    [SerializeField] private GenericInvSlot slotPrefab;

    public void OnEnable() => Rebuild();

    public void Rebuild()
    {
        if (!animalDB || !content || !slotPrefab) return;

        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        foreach (var data in animalDB.animalList)
        {
            if (data == null) continue;
            var slot = Instantiate(slotPrefab, content);

            var icon = data.GetIcon(new ResourcesLoader());
            slot.SetIcon(icon);
            slot.SetCountVisible(false);

            slot.SetOnClick(() =>
            {
                var edit = FindFirstObjectByType<EditModeController>();
                if (!edit) return;
                edit.SpawnFromPlaceable(new AnimalPlaceable(data), PlaceableCategory.Animal);
            });

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
}
