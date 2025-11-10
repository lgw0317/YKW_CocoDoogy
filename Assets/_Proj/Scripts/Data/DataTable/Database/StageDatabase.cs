using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDatabase", menuName = "GameData/StageDatabase")]
public class StageDatabase : ScriptableObject, IEnumerable<StageData>
{
    public List<StageData> stageDataList = new List<StageData>();
    public IEnumerator<StageData> GetEnumerator()
    {
        return stageDataList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
