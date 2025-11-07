using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 코코두기, 안드로이드(코코두기 주인)
/// 메인 캐릭터들은 데이터 테이블에는 없지만
/// 로비 쪽에서 사용 및 구조 통일성을 위해서 만들었습니다. 
/// 기획 쪽에서는 편하신 대로 하라 해서 흠흠.
/// </summary>
/// 
[Serializable]
public class MainCharacterData
{
    public int mainChar_id;
    public string mainChar_name;
    public MainCharacterType mainChar_type;
    public MainCharacterTag mainChar_tag;
    public string mainChar_prefab;
    public string mainChar_icon;
    public MainCharacterAcquire mainChar_acquire;
    public string mainChar_desc;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;
    public GameObject GetPrefab(IResourceLoader loader)
    {
        if (prefab == null && !string.IsNullOrEmpty(mainChar_prefab))
            prefab = loader.LoadPrefab(mainChar_prefab);
        return prefab;
    }

    public Sprite GetIcon(IResourceLoader loader)
    {
        if (icon == null && !string.IsNullOrEmpty(mainChar_icon))
            icon = loader.LoadSprite(mainChar_icon);
        return icon;
    }
}
public enum MainCharacterType
{
    CocoDoogy, Master
}
public enum MainCharacterTag
{
    basic, xmas, halloween
}
public enum MainCharacterAcquire
{
    none, quest, ingame, shop
}

[CreateAssetMenu(fileName = "MainCharacterDatabase", menuName = "GameData/MainCharacterDatabase")]
public class MainCharacterDatabase : ScriptableObject
{
    public List<MainCharacterData> mainCharDataList = new List<MainCharacterData>();
}
