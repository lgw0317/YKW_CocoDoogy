using System;

[Serializable]
public class StageProgressData
{
    public string stageId;
    public bool[] treasureCollected = new bool[3];
}