using UnityEngine;

public class GameQuit : MonoBehaviour
{
    public void QuiteGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
