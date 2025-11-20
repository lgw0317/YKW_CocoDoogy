using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public void UIAudioNormalOpen()
    {
        AudioEvents.Raise(UIKey.Normal, 0);
    }
    public void UIAudioNormalClose()
    {
        AudioEvents.Raise(UIKey.Normal, 1);
    }
    public void UIAudioNormalSelect()
    {
        AudioEvents.Raise(UIKey.Normal, 2);
    }
}
