using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[Serializable]
public class DecoData
{
    public int deco_id;
    public string deco_name;
    public string deco_prefab;
    public string deco_icon;
    public DecoType deco_type;
    public DecoTag deco_tag; 
    public DecoAcquire deco_acquire;
    public int deco_stack;
    public string deco_desc;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;

    //DataTable에 있는 변수들을 가져와서 DecoData 클래스에 선언
    //다른 DataTable도 각각의 클래스를 만들어서 DataTable에 있는 변수들을 갖게 제작

    public GameObject GetPrefab(IResourceLoader loader)
    {
        if (prefab == null && !string.IsNullOrEmpty(deco_prefab))
            prefab = loader.LoadPrefab(deco_prefab);
        return prefab;
    }

    public Sprite GetIcon(IResourceLoader loader)
    {
        if (icon == null && !string.IsNullOrEmpty(deco_icon))
            icon = loader.LoadSprite(deco_icon);
        return icon;
    }
    //Deco의 prefab과 sprite가 필요할 때 사용할 함수
}
public enum DecoType
{
    plant, strucure, furniture, prop
}
public enum DecoTag
{
    tree, water, chair, music, lite, table, street
}
public enum DecoAcquire
{
    quest, ingame, shop
}