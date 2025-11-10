using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "GameData/DialogueDatabase")]
public class DialogueDatabase : ScriptableObject, IEnumerable<DialogueData>
{
    public List<DialogueData> dialogueList = new List<DialogueData>();
    public IEnumerator<DialogueData> GetEnumerator()
    {
        return dialogueList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}