using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TreasureDatabase", menuName = "GameData/TreasureDatabase")]
public class TreasureDatabase : ScriptableObject, IEnumerable<TreasureData>
{
    public List<TreasureData> treasureList = new List<TreasureData>();
    public IEnumerator<TreasureData> GetEnumerator()
    {
        return treasureList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
