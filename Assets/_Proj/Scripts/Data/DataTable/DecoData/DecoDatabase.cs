using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecoDatabase", menuName = "GameData/DecoDatabase")]
public class DecoDatabase : ScriptableObject
{
    public List<DecoData> decoList = new List<DecoData>();
}