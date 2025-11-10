using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecoDatabase", menuName = "GameData/DecoDatabase")]
public class DecoDatabase : ScriptableObject, IEnumerable<DecoData>

{
    public List<DecoData> decoList = new List<DecoData>();
    public IEnumerator<DecoData> GetEnumerator()
    {
        return decoList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
//스크립터블 오브젝트에 DecoData를 list로 보관