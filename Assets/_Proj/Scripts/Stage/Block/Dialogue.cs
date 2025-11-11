using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    private string dialogueId;
    private bool isRead = false;

    public void Init(string id)
    {
        dialogueId = id;
        Debug.Log($"[Dialogue] Init 완료 → ID: {dialogueId}");
    }
    void Start()
    {
        if(StageUIManager.Instance.stageManager.isTest)
        {
            dialogueId = "dialogue_1_1_1";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isRead) return;
        if(other.gameObject.CompareTag("Player"))
        {
            var data = DataManager.Instance.Dialogue.GetData(dialogueId);

            //플레이어와 접촉 시 대화 다이어로그 실행
            StageUIManager.Instance.Overlay.SetActive(true);
            StageUIManager.Instance.DialoguePanel.SetActive(true);
            StageUIManager.Instance.OptionOpenButton.gameObject.SetActive(false);
            isRead = true;

            var speakData = DataManager.Instance.Speaker.GetData(data.speaker_id);

            var speakerSprite = DataManager.Instance.Speaker.GetPortrait(data.speaker_id, speakData.portrait_set_prefix);

            Color color = new Color(1, 1, 1, 0.2f);

            if (data.speaker_position == SpeakerPosition.left)
            {
                StageUIManager.Instance.DialogueSpeakerLeft.sprite = speakerSprite;
                StageUIManager.Instance.DialogueSpeakerRight.color = color;

            }
            else
            {
                StageUIManager.Instance.DialogueSpeakerRight.sprite = speakerSprite;
                StageUIManager.Instance.DialogueSpeakerLeft.color = color;
            }

            StageUIManager.Instance.DialogueNameText.text = speakData.display_name;

            var dialogueText = StageUIManager.Instance.DialogueText;
            StopAllCoroutines();
            StartCoroutine(TypeText(dialogueText, data.text, data.char_delay));
        }
    }

    // TextMeshPro 타이핑 효과 함수
    private IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, float delay)
    {
        textComponent.text = "";
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}
