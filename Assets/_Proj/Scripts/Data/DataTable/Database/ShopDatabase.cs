using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopDatabase", menuName = "GameData/ShopDatabase")]
public class ShopDatabase : ScriptableObject, IEnumerable<ShopData>
{
    public List<ShopData> shopDataList = new List<ShopData>();
    public IEnumerator<ShopData> GetEnumerator()
    {
        return shopDataList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
