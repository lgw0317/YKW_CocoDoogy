using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    private string dialogueId;

    private PlayerMovement playerMovement;

    public void Init(string id)
    {
        dialogueId = id;
        Debug.Log($"[Dialogue] Init 완료 → ID: {dialogueId}");
    }
    void Start()
    {
        if (StageUIManager.Instance.stageManager.isTest)
        {
            dialogueId = "dialogue_1_5_1";
        }
    }

    void OnTriggerEnter(Collider other)
    {
       
        if (!other.CompareTag("Player")) return;

        playerMovement = other.GetComponent<PlayerMovement>();
        playerMovement.enabled = false;

        DialogueManager.Instance.NewDialogueMethod(dialogueId);

        playerMovement.enabled = true;
    }
    
}
