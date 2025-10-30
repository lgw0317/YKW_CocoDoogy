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
    public Type deco_type;
    public Tag deco_tag; 
    public Acquire deco_acquire;
    public int deco_stack;
    public string deco_desc;

    [NonSerialized] public GameObject prefab;
    [NonSerialized] public Sprite icon;

    //DataTable에 있는 변수들을 가져와서 DecoData 클래스에 선언
    //다른 DataTable도 각각의 클래스를 만들어서 DataTable에 있는 변수들을 갖게 제작

    public GameObject GetPrefab()
    {
        if (prefab == null && !string.IsNullOrEmpty(deco_prefab))
            prefab = Resources.Load<GameObject>(deco_prefab);
        return prefab;
    }

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(deco_icon))
            icon = Resources.Load<Sprite>(deco_icon);
        return icon;
    }
    //Deco의 prefab과 sprite가 필요할 때 사용할 함수
}
public enum Type
{
    plant, strucure, furniture, fixtures
}
public enum Tag
{
    tree, flower, chair, house, lite
}
public enum Acquire
{
    quest, ingame, shop
}