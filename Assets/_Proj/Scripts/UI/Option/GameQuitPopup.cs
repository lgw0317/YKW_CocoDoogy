using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameQuitPopup : MonoBehaviour
{
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void OnQuitYes()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
