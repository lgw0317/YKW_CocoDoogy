using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "GameData/QuestDatabase")]
public class QuestDatabase : ScriptableObject, IEnumerable<QuestData>
{
    public List<QuestData> questList = new List<QuestData>();
    public IEnumerator<QuestData> GetEnumerator()
    {
        return questList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
