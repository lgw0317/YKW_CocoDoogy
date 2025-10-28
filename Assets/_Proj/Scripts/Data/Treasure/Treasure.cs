using System;
using UnityEngine;

public enum TreasureType
{
    Cap,     //인게임 재화, 상점에서 사용
    Article, //조경물, 거점에 배치해서 꾸밀 수 있는 설치아이템, 도감 활성화
    Acce,    //치장 아이템, 코코두기를 꾸밀 수 있는 치장아이템, 도감 활성화
    Relic    //유물, 세계관 관련 아이템, 도감 활성화
}

[CreateAssetMenu(fileName = "Treasure", menuName = "TreasureSO/Treasure")]

public class Treasure : ScriptableObject
{
    public int Treasure_Id; //보물 ID

    public TreasureType Treaure_Type; //보물의 종류

    public int Take_Item_ID; //아이템 ID

    public int Count; // 수량

    public GameObject prefab;

    public Sprite icon;
}