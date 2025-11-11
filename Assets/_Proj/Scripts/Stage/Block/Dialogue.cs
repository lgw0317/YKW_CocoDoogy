using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private string dialogueId;
    private bool isRead = false;

    public void Init(string id)
    {
        dialogueId = id;
        Debug.Log($"[Dialogue] Init 완료 → ID: {dialogueId}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (isRead) return;
        if(other.gameObject.CompareTag("Player"))
        {
            dialogueId = "dialogue_1_1_1";
            var data = DataManager.Instance.Dialogue.GetData(dialogueId);

            //플레이어와 접촉 시 대화 다이어로그 실행
            StageUIManager.Instance.DialoguePanel.SetActive(true);
            StageUIManager.Instance.OptionOpenButton.gameObject.SetActive(false);
            isRead = true;

            var speakerSprite = DataManager.Instance.Speaker.GetPortrait(data.speaker_id, data.emotion.ToString());

            StageUIManager.Instance.DialogueSpeakerLeft.sprite = speakerSprite;
        }
    }
}
