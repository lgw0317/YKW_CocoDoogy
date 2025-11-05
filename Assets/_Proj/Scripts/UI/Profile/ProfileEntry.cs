using UnityEngine;

[System.Serializable]
public class ProfileEntry
{
    public readonly int Id;
    public readonly Sprite Icon;
    public readonly string Category;

    public ProfileEntry(int id, Sprite icon, string category)
    {
        Id = id;
        Icon = icon;
        Category = category;
    }
}