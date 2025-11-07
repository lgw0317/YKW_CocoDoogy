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
    public string codex_name;

    [NonSerialized] public Sprite codexDisplay;
    [NonSerialized] public Sprite codexIcon;

    public Sprite GetCodexDisplay(IResourceLoader loader)
    {
        if (codexDisplay == null && !string.IsNullOrEmpty(codex_display))
            codexDisplay = loader.LoadSprite(codex_display);
        return codexDisplay;
    }

    public Sprite GetCodexIcon(IResourceLoader loader)
    {
        if (codexIcon == null && !string.IsNullOrEmpty(codex_icon))
            codexIcon = loader.LoadSprite(codex_icon);
        return codexIcon;
    }
}

public enum CodexType
{
    deco, costume, animal, home, artifact
}
