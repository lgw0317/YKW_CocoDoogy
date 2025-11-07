using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoodsDatabase", menuName = "GameData/GoodsDatabase")]
public class GoodsDatabase : ScriptableObject
{
    public List<GoodsData> goodsList = new List<GoodsData>();
}
