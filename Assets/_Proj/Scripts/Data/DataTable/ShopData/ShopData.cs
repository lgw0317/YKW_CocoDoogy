using System;
using UnityEngine;

[Serializable]
public class ShopData
{
    public int shop_id;
    public string shop_name;
    public string shop_icon;
    public string shop_desc;
    public int shop_item;
    public ShopType shop_type;
    public int shop_price;

    [NonSerialized] public Sprite icon;

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(shop_icon))
            icon = Resources.Load<Sprite>(shop_icon);
        return icon;
    }
}
public enum ShopType
{
    cap, coin
}
