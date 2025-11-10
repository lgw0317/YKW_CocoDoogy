using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactData", menuName = "GameData/ArtifactData")]
public class ArtifactDatabase : ScriptableObject, IEnumerable<ArtifactData>
{
    public List<ArtifactData> artifactList = new List<ArtifactData>();

    public IEnumerator<ArtifactData> GetEnumerator()
    {
        return artifactList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
