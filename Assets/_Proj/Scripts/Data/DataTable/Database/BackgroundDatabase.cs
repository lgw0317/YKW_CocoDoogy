using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "GameData/BackgroundDatabase")]
public class BackgroundDatabase : ScriptableObject, IEnumerable<BackgroundData>
{
    public List<BackgroundData> bgList = new List<BackgroundData>();

    public IEnumerator<BackgroundData> GetEnumerator()
    {
        return bgList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
