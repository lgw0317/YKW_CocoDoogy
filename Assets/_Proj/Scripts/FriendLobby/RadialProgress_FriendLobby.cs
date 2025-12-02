using UnityEngine;

public class RadialProgress_FriendLobby : RadialProgress
{
    protected override void OnEnable()
    {
        ApplyProgress(FriendLobbyManager.Instance.Friend);

        currentFill = 0f;
        displayedValue = 0f;

        if (fillImage)
            fillImage.fillAmount = 0f;
    }
}
