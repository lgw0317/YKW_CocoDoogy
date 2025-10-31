using System;
using UnityEngine;

[Serializable]
public class StageData
{
    public string stage_id;
    public string stage_name;
    public string stage_img;
    public string stage_desc;
    public string map_id;
    public string start_cutscene;
    public string start_talk;
    public string end_talk;
    public string end_cutscene;
    public string treasure_01_id;
    public string treasure_02_id;
    public string treasure_03_id;

    [NonSerialized] public Sprite icon;

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(stage_img))
            icon = Resources.Load<Sprite>(stage_img);
        return icon;
    }

    //TODO : cutscene 관련 변수 및 Get함수 추가필요
    //cutscene은 어떻게 실행시킬지 확인필요
}
