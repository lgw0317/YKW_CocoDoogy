using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// TODO : 구글 로그인 기능 여기에 추가
public class OptionPanelController : MonoBehaviour
{
    public OptionPanelController Instance;

    [Header("Popups")]
    [SerializeField] private AccountLinkAskPopup accountLinkAskPopup;
    [SerializeField] private LogoutPopup logoutPopup;
    [SerializeField] private GameInfoPopup gameInfoPopup;
    [SerializeField] private GameQuitPopup gameQuitPopup;

    [SerializeField] private GameObject dim;

    [Header("LinkPopups")]
    public GameObject accountLinkSuccessPopup;
    public GameObject accountLinkFailPopup;

    [SerializeField] private Button btnLinkAccount;
    [SerializeField] private Button btnLogout;
    [SerializeField] private Button btnGameInfo;
    [SerializeField] private Button btnGameQuit;


    void Awake()
    {
        if (accountLinkAskPopup) accountLinkAskPopup.gameObject.SetActive(false);
        if (logoutPopup) logoutPopup.gameObject.SetActive(false);
        if (gameInfoPopup) gameInfoPopup.gameObject.SetActive(false);
        if (gameQuitPopup) gameQuitPopup.gameObject.SetActive(false);
        if (dim) dim.SetActive(false);
        if (dim) dim.SetActive(false);

        if (btnLinkAccount)
        { 
            btnLinkAccount.onClick.AddListener(() => accountLinkAskPopup.Open());
            //파이어베이스 로그인 상태가 게스트(익명)유저냐 아니냐에 따라 링크시도버튼의 인터랙터블을 바꿔줌.
            btnLinkAccount.interactable = FirebaseManager.Instance.IsGuest;
        }
        if (btnLogout) btnLogout.onClick.AddListener(() => logoutPopup.Open());
        if (btnGameInfo) btnGameInfo.onClick.AddListener(() => gameInfoPopup.Open());
        if (btnGameQuit) btnGameQuit.onClick.AddListener(() => gameQuitPopup.Open());
    }

    public void LinkAccountOpen()
    {
        accountLinkAskPopup.Open();
    }

    public void GameInfoOpen()
    {
        gameInfoPopup.Open();
    }

    public void LogoutOpen()
    {
        logoutPopup.Open();
    }

    public void GameQuitOpen()
    {
        gameQuitPopup.Open();
    }

    public async void TryGoogleLogin()
    {
        
        if (await FirebaseManager.Instance.GoogleLogin())
        //위의 것을 await하게 됨. 대기 후 반환되는 결과가 true라면 구글로그인(구글연동) 성공, false라면 구글로그인 실패임.
        {
            OnGoogleLoginSuccess();
        }
        else
        {
            OnGoogleLoginFail();
        }
            //SceneManager.LoadScene("Title");
    }


    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
    // 로그인 성공/실패 콜백
    public void OnGoogleLoginSuccess()
    {
        accountLinkSuccessPopup.SetActive(true);
            btnLinkAccount.interactable = FirebaseManager.Instance.IsGuest;
    }

    public void OnGoogleLoginFail()
    {
        accountLinkFailPopup.SetActive(true);
    }
}
