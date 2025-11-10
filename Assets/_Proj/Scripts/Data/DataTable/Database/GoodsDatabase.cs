using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoodsDatabase", menuName = "GameData/GoodsDatabase")]
public class GoodsDatabase : ScriptableObject, IEnumerable<GoodsData>
{
    public List<GoodsData> goodsList = new List<GoodsData>();
    public IEnumerator<GoodsData> GetEnumerator()
    {
        return goodsList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
