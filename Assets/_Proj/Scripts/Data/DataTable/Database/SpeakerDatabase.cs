using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerDatabase", menuName = "GameData/SpeakerDatabase")]
public class SpeakerDatabase : ScriptableObject, IEnumerable<SpeakerData>
{
    // SpeakerData (화자 정보)의 리스트를 저장
    public List<SpeakerData> speakerList = new List<SpeakerData>();
    public IEnumerator<SpeakerData> GetEnumerator()
    {
        return speakerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}