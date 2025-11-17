using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [Header("Popups")]
    [SerializeField] private AccountLinkPopup accountLinkPopup;
    [SerializeField] private LogoutPopup logoutPopup;
    [SerializeField] private GameInfoPopup gameInfoPopup;
    [SerializeField] private GameQuitPopup gameQuitPopup;

    [SerializeField] private Button btnLinkAccount;
    [SerializeField] private Button btnLogout;
    [SerializeField] private Button btnGameInfo;
    [SerializeField] private Button btnGameQuit;

    void Awake()
    {
        if (accountLinkPopup) accountLinkPopup.gameObject.SetActive(false);
        if (logoutPopup) logoutPopup.gameObject.SetActive(false);
        if (gameInfoPopup) gameInfoPopup.gameObject.SetActive(false);
        if (gameQuitPopup) gameQuitPopup.gameObject.SetActive(false);

        if (btnLinkAccount) btnLinkAccount.onClick.AddListener(() => accountLinkPopup.Open());
        if (btnLogout) btnLogout.onClick.AddListener(() => logoutPopup.Open());
        if (btnGameInfo) btnGameInfo.onClick.AddListener(() => gameInfoPopup.Open());
        if (btnGameQuit) btnGameQuit.onClick.AddListener(() => gameQuitPopup.Open());
    }

    public void LinkAccountOpen()
    {
        accountLinkPopup.Open();
    }

    public void GameInfoOpen()
    {
        gameInfoPopup.Open();
    }

    private void LogoutOpen()
    {
        logoutPopup.Open();
    }

    public void GameQuitOpen()
    {
        gameQuitPopup.Open();
    }
}
