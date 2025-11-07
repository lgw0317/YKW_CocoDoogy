using System;
using UnityEngine;

[Serializable]
public class GoodsData
{
    public int goods_id;
    public string goods_name;
    public string goods_icon;
    public string goods_desc;

    [NonSerialized] public Sprite icon;
    public Sprite GetIcon(IResourceLoader loader)
    {
        if (icon == null && !string.IsNullOrEmpty(goods_icon))
            icon = loader.LoadSprite(goods_icon);
        return icon;
    }
}
