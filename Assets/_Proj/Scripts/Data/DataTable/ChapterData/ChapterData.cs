using System;
using UnityEngine;

[Serializable]
public class ChapterData
{
    public string chapter_id;
    public string chapter_name;
    public string chapter_desc;
    public string chapter_img;
    public string[] chapter_staglist;
    public string chapter_bg;

    [NonSerialized] public Sprite chapterIcon;
    [NonSerialized] public Sprite chapterBgIcon;

    public Sprite GetChapterIcon()
    {
        if (chapterIcon == null && !string.IsNullOrEmpty(chapter_img))
            chapterIcon = Resources.Load<Sprite>(chapter_img);
        return chapterIcon;
    }

    public Sprite GetChapterBgIcon()
    {
        if (chapterBgIcon == null && !string.IsNullOrEmpty(chapter_img))
            chapterBgIcon = Resources.Load<Sprite>(chapter_img);
        return chapterBgIcon;
    }
}
