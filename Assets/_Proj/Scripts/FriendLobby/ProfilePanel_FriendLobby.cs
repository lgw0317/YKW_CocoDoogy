using Firebase.Auth;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel_FriendLobby : MonoBehaviour
{
    private UserData Friend => FriendLobbyManager.Instance.Friend;
    private string Uid => FriendLobbyManager.Instance.Uid;
    [Header("Basic Info")]
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private TMP_Text uidText;
    [SerializeField] private TMP_Text joindateText;
    [SerializeField] private TMP_Text totalLikeText;

    [Header("Big Profile Image")]
    [SerializeField] private Image profileBigImage;

    [Header("Museum")]
    [SerializeField] private ProfileFavoriteIcon animalIcon;
    [SerializeField] private ProfileFavoriteIcon decoIcon;
    [SerializeField] private ProfileFavoriteIcon costumeIcon;
    [SerializeField] private ProfileFavoriteIcon artifactIcon;
    public void Open()
    {
        UIPanelAnimator.Open(gameObject);
    }

    private void OnEnable()
    {
        SetupFriendInfo();

        animalIcon.Initialize(null, ProfileType.animal, Friend.master[ProfileType.animal]);
        decoIcon.Initialize(null, ProfileType.deco, Friend.master[ProfileType.deco]);
        costumeIcon.Initialize(null, ProfileType.costume, Friend.master[ProfileType.costume]);
        artifactIcon.Initialize(null, ProfileType.artifact, Friend.master[ProfileType.artifact]);
    }


    private void SetupFriendInfo()
    {
        //var user = _auth.CurrentUser;
        

        if (nicknameText) nicknameText.text = Friend.master.nickName ?? "-";
        if (uidText) uidText.text = Uid ?? "-";
        if (joindateText)
        {
            var createdAt = Friend.master.createdAt;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(createdAt);
            string formatted = dateTimeOffset.ToLocalTime().ToString("yyyy/MM/dd");
            joindateText.text = formatted;
        }
        profileBigImage.sprite = DataManager.Instance.Profile.GetIcon(Friend.master[ProfileType.icon]);
        totalLikeText.text = $"누적 로비 좋아요  :  {Friend.master.totalLikes}";


    }

    public void Close()
    {
        UIPanelAnimator.Close(gameObject);
    }
    
}
