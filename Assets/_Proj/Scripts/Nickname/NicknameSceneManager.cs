using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NicknameSceneManager : MonoBehaviour
{
    [HideInInspector]
    public bool isNicknameChangedSuccessfully;

    [SerializeField] TMP_InputField nicknameInputField;
    [SerializeField] TMP_Text logText;
    [SerializeField] Button confirmButton;
    void Awake()
    {
        confirmButton.onClick.AddListener(() => OnConfirmButtonClick());
    }

    public async void OnConfirmButtonClick()
    {
        confirmButton.interactable = false;
        if (await FirebaseManager.Instance.CheckIfNameReservedAndReset(nicknameInputField.text, ShowLog))
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => ToMainScene());
            confirmButton.interactable = true;
            ShowLog("<color=green>닉네임 설정 완료!</color>\n버튼을 누르면 로비로 이동합니다.");
        }
        else
        {
            confirmButton.interactable = true;
            nicknameInputField.text = "";
        }
    }

    private void ShowLog(string message)
    {
        if (logText.alpha < 1)
        {
            logText.alpha = 1;
        }
        logText.text = message; 
        
    }

    public void ToMainScene()
    {
        confirmButton.interactable = false;
        SceneManager.LoadScene("Main");
    }


}
