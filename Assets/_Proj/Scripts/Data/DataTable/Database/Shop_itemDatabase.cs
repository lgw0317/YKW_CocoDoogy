using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Shop_itemDatabase", menuName = "GameData/Shop_itemDatabase")]
public class Shop_itemDatabase : ScriptableObject, IEnumerable<Shop_itemData>
{
    public List<Shop_itemData> shopItemList = new List<Shop_itemData>();
    public IEnumerator<Shop_itemData> GetEnumerator()
    {
        return shopItemList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
