using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterDatabase", menuName = "GameData/ChapterDatabase")]
public class ChapterDatabase : ScriptableObject, IEnumerable<ChapterData>
{
    public List<ChapterData> chapterList = new List<ChapterData>();

    public IEnumerator<ChapterData> GetEnumerator()
    {
        return chapterList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
