using UnityEngine;

public class AccountLinkPopup : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_LinkGoogle()
    {
        // TODO: Firebase / Google 연동 시도
        Debug.Log("Google 연동 시도");

       
    }
}
