using Firebase.Auth;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanelController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] public ProfileService profileService;
    [SerializeField] private QuestDatabase questDatabase;

    [Header("Top Info (optional)")]
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private TMP_Text uidText;
    [SerializeField] private TMP_Text joindateText;
    [SerializeField] private TMP_Text totalLikeText;

    [Header("Big Profile Image")]
    [SerializeField] private Image profileBigImage;

    [Header("Category Icons (item slots)")]
    [SerializeField] private ProfileFavoriteIcon animalIcon;
    [SerializeField] private ProfileFavoriteIcon decoIcon;
    [SerializeField] private ProfileFavoriteIcon costumeIcon;
    [SerializeField] private ProfileFavoriteIcon artifactIcon;

    [Header("Popups")]
    [SerializeField] private ProfileItemSelector itemSelectorPopup;   // 아이템용 UI
    [SerializeField] private ProfileIconSelector iconSelectorPopup;   // 프로필아이콘용 UI

    //12.05mj
    [Header("Red Dot")]
    [SerializeField] private GameObject profilePanelRedDot;   // 프로필 패널 안 빨간점

    // 외부(헤더)에 있는 프로필 버튼이 여기 구독해서 아이콘만 바꾸면 됨
    public System.Action<Sprite, int> OnProfileIconChanged;

    //이제 이 클래스는 파이어베이스 Auth를 전혀 몰라도 됩니다.
    //Firebase Auth가 필요한 부분은 uid 외에는 없음. 따라서 임시 변수로 돌립니다.
    //private FirebaseAuth _auth;

    private void Start()
    {
        UserData.Local.master.onMasterUpdate += SetupUserInfo;
    }

    private void OnDestroy()
    {
        UserData.Local.master.onMasterUpdate -= SetupUserInfo;
    }
    private void OnEnable()
    {
        SetupUserInfo();
        SetupCategoryIcons();
        UIPanelAnimator.Open(gameObject);

        // 12.05mj 프로필 빨간점 구독
        ProfileRedDotManager.OnStateChanged += ApplyProfileRedDot;
        ApplyProfileRedDot(ProfileRedDotManager.Current);
    }
    //12.05mj
    private void OnDisable()
    {
        ProfileRedDotManager.OnStateChanged -= ApplyProfileRedDot;
    }

    private void ApplyProfileRedDot(ProfileRedDotState state)
    {
        if (profilePanelRedDot)
            profilePanelRedDot.SetActive(state.hasNewIcon);
    }
    public void Close()
    {

        //LSH 추가 1125
        AudioEvents.Raise(UIKey.Normal, 1);

        if (itemSelectorPopup)
            itemSelectorPopup.Close();
        if (iconSelectorPopup)
            iconSelectorPopup.Close();

        UIPanelAnimator.Close(gameObject);
    }

    private void SetupUserInfo()
    {
        //var user = _auth.CurrentUser;
        var user = UserData.Local.master;

        if (nicknameText) nicknameText.text = UserData.Local.master.nickName ?? "-";
        if (uidText) uidText.text = FirebaseAuth.DefaultInstance.CurrentUser?.UserId ?? "-";
        if (joindateText)
        {
            var createdAt = UserData.Local.master.createdAt;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(createdAt);
            string formatted = dateTimeOffset.ToLocalTime().ToString("yyyy/MM/dd");
            joindateText.text = formatted;
        }
        profileBigImage.sprite = DataManager.Instance.Profile.GetIcon(UserData.Local.master[ProfileType.icon]);
        totalLikeText.text = $"누적 로비 좋아요  :  {UserData.Local.master.totalLikes}";


    }

    private void SetupCategoryIcons()
    {
        InitCategory(ProfileType.animal, animalIcon);
        InitCategory(ProfileType.deco, decoIcon);
        InitCategory(ProfileType.costume, costumeIcon);
        InitCategory(ProfileType.artifact, artifactIcon);
    }

    private void InitCategory(ProfileType type, ProfileFavoriteIcon target)
    {
        if (!target || !profileService) return;

        target.Initialize(this, type, UserData.Local.master[type]);

        //if (type == ProfileType.animal)
        //{
        //    int startingId = 30000;
        //    foreach(var entry in entries)
        //    {
        //        entry.Init(++startingId, profileService);
        //    }
        //}
        //if (type == ProfileType.deco)
        //{
        //    int startingId = 10000;
        //    foreach (var entry in entries)
        //    {
        //        entry.Init(++startingId, profileService);
        //    }
        //}
        //if (type == ProfileType.costume)
        //{
        //    int startingId = 20000;
        //    foreach (var entry in entries)
        //    {
        //        entry.Init(++startingId, profileService);
        //    }
        //}
        //if (type == ProfileType.artifact)
        //{
        //    int startingId = 50000;
        //    foreach (var entry in entries)
        //    {
        //        entry.Init(++startingId, profileService);
        //    }
        //}                    
    }

    public void OpenItemPopup(ProfileType type, ProfileFavoriteIcon caller)
    {
        if (!itemSelectorPopup) return;
        //LSH 추가 1125
        AudioEvents.Raise(UIKey.Normal, 3);
        //
        itemSelectorPopup.gameObject.SetActive(true);
        itemSelectorPopup.Open(type, caller, this);
    }

    // 프로필아이콘 변경 버튼 눌렀을 때
    public void OpenProfileIconPopup()
    {
        if (!iconSelectorPopup) return;

        //LSH 추가 1125
        AudioEvents.Raise(UIKey.Normal, 3);
        //
        //iconSelectorPopup.gameObject.SetActive(true);
        iconSelectorPopup.Open(this);
    }

    public void CloseIconPopup()
    {
        iconSelectorPopup.Close();
    }

    //해금
    public bool IsIconUnlocked(int iconId)
    {
        // 기본아이콘은 기본지급 아이콘 두가지 중 선택 가능
        if (iconId == 120001 || iconId == 120002)
            return true;

        var qData = UserData.Local.quest;

        foreach (var q in questDatabase.questList)
        {
            if (q.quest_type != QuestType.achievements)
                continue;

            if (q.quest_reward_item == iconId)
            {
                if (qData.rewarded.Contains(q.quest_id))
                    return true;
            }
        }

        return false;
    }

    public void EquipProfileIcon(int iconId)
    {
        if (!IsIconUnlocked(iconId))
        {
            Debug.LogWarning("잠긴 아이콘은 선택 불가");
            return;
        }

        UserData.Local.master[ProfileType.icon] = iconId;
        UserData.Local.Save();

        Sprite newIcon = DataManager.Instance.Profile.GetIcon(iconId);

        profileBigImage.sprite = newIcon;
        OnProfileIconChanged?.Invoke(newIcon, iconId);

        SetupUserInfo();
    }

    // 프로필 아이콘 저장(Firebase)
    // ???? 이건 등록된 프로필 아이콘을 저장할 방법을 궁리해봐야 할 듯. 프로필 아이콘은 유저가 업로드할 이미지들인지? 게임 리소스로 존재할 것인지?
    // 단, 유저가 직접 업로드하는 이미지라면 일반적인 방법으로는 프로필 아이콘 설정이 불가능함.
    //TODO: 이거 고쳐야 함. 잘못된 노드를 firebase에 만들고 있음.
    //public void SaveEquipped(ProfileType type, int itemId)
    //{
    //    var user = FirebaseAuth.DefaultInstance.CurrentUser;
    //    if (user == null) return;

    //    FirebaseDatabase.DefaultInstance
    //        .GetReference($"users/{user.UserId}/profile/equipped/{type}")
    //        .SetValueAsync(itemId);
    //}

    // 프로필 아이콘 바뀌었을 때: 패널 안 + 외부에 알림
    public void NotifyProfileIconChanged(Sprite icon, int itemId)
    {
        // 패널 안에서 다른 UI들도 바꾸고 싶으면 여기서 처리
        OnProfileIconChanged?.Invoke(icon, itemId);
        profileBigImage.sprite = icon;
    }


}