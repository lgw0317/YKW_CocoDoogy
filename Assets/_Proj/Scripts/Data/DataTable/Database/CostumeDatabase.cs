using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CostumeDatabase", menuName = "GameData/CostumeDatabase")]
public class CostumeDatabase : ScriptableObject, IEnumerable<CostumeData>
{
    public List<CostumeData> costumeList = new List<CostumeData>();
    public IEnumerator<CostumeData> GetEnumerator()
    {
        return costumeList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
