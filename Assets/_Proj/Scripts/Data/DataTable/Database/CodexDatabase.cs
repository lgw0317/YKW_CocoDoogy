using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CodexDatabase", menuName = "Scriptable Objects/CodexDatabase")]
public class CodexDatabase : ScriptableObject, IEnumerable<CodexData>
{
    public List<CodexData> codexList = new List<CodexData>();
    public IEnumerator<CodexData> GetEnumerator()
    {
        return codexList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
