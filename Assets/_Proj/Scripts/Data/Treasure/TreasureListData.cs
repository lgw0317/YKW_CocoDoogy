using UnityEngine;

[CreateAssetMenu(fileName = "TreasureListData", menuName = "TreasureSO/TreasureListData")]
public class TreasureListData : ScriptableObject
{
    public string typeName;
    public TreasureData[] treasures;
}
