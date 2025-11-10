using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalDatabase", menuName = "GameData/AnimalDatabase")]
public class AnimalDatabase : ScriptableObject, IEnumerable<AnimalData>
{
    public List<AnimalData> animalList = new List<AnimalData>();

    public IEnumerator<AnimalData> GetEnumerator()
    {
        return animalList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
