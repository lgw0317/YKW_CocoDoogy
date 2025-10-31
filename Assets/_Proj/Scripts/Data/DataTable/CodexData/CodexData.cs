using System;
using UnityEngine;

[Serializable]
public class CodexData
{
    public string codex_id;
    public CodexType codex_type;
    public int item_id;
    public string codex_lore;
    public string codex_display;
    public string codex_icon;

    [NonSerialized] public Sprite codexDisplay;
    [NonSerialized] public Sprite codexIcon;

    public Sprite GetCodexDisplay()
    {
        if (codexDisplay == null && !string.IsNullOrEmpty(codex_display))
            codexDisplay = Resources.Load<Sprite>(codex_display);
        return codexDisplay;
    }

    public Sprite GetCodexIcon()
    {
        if (codexIcon == null && !string.IsNullOrEmpty(codex_icon))
            codexIcon = Resources.Load<Sprite>(codex_icon);
        return codexIcon;
    }
}

public enum CodexType
{
    deco, costume, animal, home, artifact
}
