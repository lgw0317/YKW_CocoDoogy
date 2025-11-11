using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomeDatabase", menuName = "GameData/HomeDatabase")]
public class HomeDatabase : ScriptableObject, IEnumerable<HomeData>
{
    public List<HomeData> homeList = new List<HomeData>();
    public IEnumerator<HomeData> GetEnumerator()
    {
        return homeList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
