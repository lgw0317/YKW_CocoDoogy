using UnityEngine;

public class LogoutPopup : MonoBehaviour
{

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
