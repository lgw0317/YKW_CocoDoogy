using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageUIManager : MonoBehaviour
{
    [Header("Button")]
    public Button OptionOpenButton;
    public Button OptionCloseButton;
    public Button RetryButton;
    public Button QuitButton;

    [Header("OptionObject")]
    public GameObject OptionImg;

    private string map_id;

    void Awake()
    {
        OptionOpenButton.onClick.AddListener(OptionOpen);
        OptionCloseButton.onClick.AddListener(OptionClose);
        RetryButton.onClick.AddListener(Retry);
        QuitButton.onClick.AddListener(Quit);
        OptionImg.SetActive(false);
    }

    void OptionOpen()
    {
        OptionImg.SetActive(true);
        OptionOpenButton.gameObject.SetActive(false);
    }

    void OptionClose()
    {
        OptionImg.SetActive(false);
        OptionOpenButton.gameObject.SetActive(true);
    }

    void Retry()
    {
        //Todo : 챕터에 따라 분기
        SceneManager.LoadScene("Chapter1_StageScene_TESTONLY");
    }

    void Quit()
    {
        //Todo : 챕터에 따라 스테이지 선택화면 분기
        SceneManager.LoadScene("Lobby");
    }
}