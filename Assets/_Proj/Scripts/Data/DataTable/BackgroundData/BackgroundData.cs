using System;
using UnityEngine;

[Serializable]
public class BackgroundData
{
    public int bg_id;
    public string bg_name;
    public BackgroundTag bg_tag;
    public string bg_icon;
    public string bg_skybox;
    public string bg_desc;

    [NonSerialized] public Material material;
    [NonSerialized] public Sprite icon;
    public Material GetMaterial()
    {
        if (material == null && !string.IsNullOrEmpty(bg_skybox))
            material = Resources.Load<Material>(bg_skybox);
        return material;
    }

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(bg_icon))
            icon = Resources.Load<Sprite>(bg_icon);
        return icon;
    }
}

public enum BackgroundTag
{
    basic, xmas, halloween, thanksgiving_day
}
