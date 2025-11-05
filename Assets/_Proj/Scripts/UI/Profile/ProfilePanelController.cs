using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;

public class ProfilePanelController : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private ProfileIconController profileIcon;
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private TMP_Text uidText;
    [SerializeField] private TMP_Text joinDateText;

    [Header("Progress")]
    [SerializeField] private RadialProgress artifactProgress;
    [SerializeField] private RadialProgress stageProgress;
    [SerializeField] private RadialProgress codexProgress;
    [SerializeField] private RadialProgress achievementProgress;

    [Header("Favorite Icons")]
    [SerializeField] private ProfileFavoriteIcon animalIcon;
    [SerializeField] private ProfileFavoriteIcon decoIcon;
    [SerializeField] private ProfileFavoriteIcon costumeIcon;
    [SerializeField] private ProfileFavoriteIcon artifactIcon;

    [Header("Popup")]
    [SerializeField] private ProfileItemSelector favoriteIconPopup;  
    [SerializeField] private ProfileIconSelector profileIconPopup;   

    [Header("Profile Source")]
    [SerializeField] private ProfileSource profileSource;         

    private FirebaseAuth _auth;

    private void Awake()
    {
        _auth = FirebaseAuth.DefaultInstance;
    }

    private void OnEnable()
    {
        LoadUserInfo();
        RefreshProgress();
        SetupIcons();
    }

    private void LoadUserInfo()
    {
        var user = _auth.CurrentUser;
        if (nicknameText != null) nicknameText.text = user?.DisplayName ?? "-";
        if (uidText != null) uidText.text = user?.UserId ?? "-";
        if (joinDateText != null) joinDateText.text = "-";
    }

    private void RefreshProgress()
    {
        if (artifactProgress != null)
            artifactProgress.SetProgress(12, 40);
        if (stageProgress != null)
            stageProgress.SetProgressPercent(0.35f);
        if (codexProgress != null)
            codexProgress.SetProgressPercent(0.58f);
        if (achievementProgress != null)
            achievementProgress.SetProgressPercent(0.72f);
    }

    private void SetupIcons()
    {
        if (profileSource == null) return;

        var all = profileSource.GetAll();
        if (all == null || all.Count == 0) return;

        SetIconForCategory(all, "동물친구", animalIcon);
        SetIconForCategory(all, "조경품", decoIcon);
        SetIconForCategory(all, "치장품", costumeIcon);
        SetIconForCategory(all, "유물", artifactIcon);
        SetIconForCategory(all, "프로필 선택", profileIcon);
    }

    private void SetIconForCategory(IReadOnlyList<ProfileEntry> allEntries, string category, object target)
    {
        ProfileEntry entry = null;
        for (int i = 0; i < allEntries.Count; i++)
        {
            if (allEntries[i].Category == category)
            {
                entry = allEntries[i];
                break;
            }
        }
        if (entry == null) return;

        switch (target)
        {
            case ProfileFavoriteIcon fav:
                // 초기 슬롯 세팅
                fav.Initialize(this, entry.Icon, category, entry.Id);
                break;
            case ProfileIconController controller:
                controller.Init(this, entry.Icon);
                break;
        }
    }

    // 팝업 열기
    public void OpenFavoriteIconPopup(string category, ProfileFavoriteIcon source)
    {
        favoriteIconPopup?.Open(category, source, this);
    }

    public void OpenProfileIconPopup(ProfileIconController source)
    {
        profileIconPopup?.Open(source, this);
    }

    // 목록 제공
    public List<ProfileOwnedItemData> GetOwnedItemsByCategory(string category)
    {
        var list = new List<ProfileOwnedItemData>();
        var entries = profileSource?.GetByCategory(category);
        if (entries == null) return list;

        foreach (var e in entries)
        {
            list.Add(new ProfileOwnedItemData
            {
                category = e.Category,
                itemId = e.Id,
                icon = e.Icon
            });
        }
        return list;
    }

    public List<ProfileOwnedItemData> GetProfileIconItems()
    {
        var list = new List<ProfileOwnedItemData>();
        var entries = profileSource?.GetByCategory("프로필 선택");
        if (entries == null) return list;

        foreach (var e in entries)
        {
            list.Add(new ProfileOwnedItemData
            {
                category = e.Category,
                itemId = e.Id,
                icon = e.Icon
            });
        }
        return list;
    }

    // 장착 처리
    public Sprite EquipItem(string category, int itemId)
    {
        var entries = profileSource?.GetByCategory(category);
        if (entries == null) return null;

        foreach (var e in entries)
        {
            if (e.Id == itemId)
                return e.Icon;
        }
        return null;
    }

    public Sprite EquipProfileIcon(int itemId)
    {
        var entries = profileSource?.GetByCategory("프로필 선택");
        if (entries == null) return null;

        foreach (var e in entries)
        {
            if (e.Id == itemId)
                return e.Icon;
        }
        return null;
    }

    [System.Serializable]
    public class ProfileOwnedItemData
    {
        public string category;
        public int itemId;
        public Sprite icon;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}