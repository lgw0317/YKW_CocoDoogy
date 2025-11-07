using System;
using UnityEngine;

[Serializable]
public class TreasureData
{
    public string treasure_id;
    public TreasureType treasureType;
    public int reward_id;
    public int count;
    public string view_codex_id;
    public string coco_coment;
}
public enum TreasureType 
{
    coin, cap, deco, costume, artifact
}
