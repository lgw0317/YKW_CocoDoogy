using System;
using UnityEngine;

[Serializable]
public class CostumeData
{
    public int costume_id;
    public string costume_name;
    public string costume_prefab;
    public string costume_icon;
    public CostumePart cos_part;
    public CostumeTag cos_tag;
    public CostumeAcquire cos_acquire;
    public string costume_desc;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;

    public GameObject GetPrefab()
    {
        if (prefab == null && !string.IsNullOrEmpty(costume_prefab))
            prefab = Resources.Load<GameObject>(costume_prefab);
        return prefab;
    }

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(costume_icon))
            icon = Resources.Load<Sprite>(costume_icon);
        return icon;
    }
}

public enum CostumePart
{
    head, body, foot, tail
}
public enum CostumeTag
{
    basic, xmas, halloween
}
public enum CostumeAcquire
{
    quest, ingame, shop
}

