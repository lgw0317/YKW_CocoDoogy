using System;
using UnityEngine;

[Serializable]
public class Profile_iconData
{
    public int icon_id;
    public string icon_name;
    public string icon_image;
    public IconAcquire icon_acquire;
    public string icon_desc;

    [NonSerialized] public Sprite icon;

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(icon_image))
            icon = Resources.Load<Sprite>(icon_image);
        return icon;
    }
}
public enum IconAcquire
{
    quest, ingame, shop
}
