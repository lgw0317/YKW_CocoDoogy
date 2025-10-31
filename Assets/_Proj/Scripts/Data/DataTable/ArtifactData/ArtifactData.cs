using System;
using UnityEngine;

[Serializable]
public class ArtifactData
{
    public int artifact_id;
    public string artifact_name;
    public string artifact_icon;
    public ArtifactType artifact_type;
    public string artifact_desc;

    [NonSerialized] public Sprite icon;

    public Sprite GetIcon()
    {
        if (icon == null && !string.IsNullOrEmpty(artifact_icon))
            icon = Resources.Load<Sprite>(artifact_icon);
        return icon;
    }
}
public enum ArtifactType
{
    paper, oldstuff, electronics
}
