using System;
using UnityEngine;

[Serializable]
public class AnimalData
{
    public int animal_id;
    public string animal_name;
    public AnimalType animal_type;
    public AnimalTag animal_tag;
    public string animal_prefab;
    public string animal_icon;
    public AnimalAcquire animal_acquire;
    public string animal_desc;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;
    public GameObject GetPrefab()
    {
        if (prefab == null && !string.IsNullOrEmpty(animal_prefab))
            prefab = Resources.Load<GameObject>(animal_prefab);
        return prefab;
    }

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(animal_icon))
            icon = Resources.Load<Sprite>(animal_icon);
        return icon;
    }
}
public enum AnimalType
{
    dog, pig, bird, rat
}
public enum AnimalTag
{
    basic, xmas, halloween
}
public enum AnimalAcquire
{
    quest, ingame, shop
}
