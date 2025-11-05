using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileIconSelector : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Transform slotParent;
    [SerializeField] private ProfileSlot slotPrefab;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text titleText;

    private int selectedItemId = -1;
    private ProfileIconController currentTargetIcon;
    private ProfilePanelController parentController;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);

        if (applyButton != null)
            applyButton.onClick.AddListener(Apply);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

    }

    public void Open(ProfileIconController targetIcon, ProfilePanelController parent)
    {
        if (root != null)
            root.SetActive(true);

        currentTargetIcon = targetIcon;
        parentController = parent;

        if (titleText != null)
            titleText.text = "프로필 선택";

        BuildList();
    }

    private void BuildList()
    {
        foreach (Transform t in slotParent)
            Destroy(t.gameObject);

        List<ProfilePanelController.ProfileOwnedItemData> iconList =
            parentController != null
            ? parentController.GetProfileIconItems()
            : null;

        if (iconList == null || iconList.Count == 0)
        {
            selectedItemId = -1;
            return;
        }

        foreach (var item in iconList)
        {
            var slot = Instantiate(slotPrefab, slotParent);
            slot.Bind(item.itemId, item.icon, OnSlotSelected);
        }

        selectedItemId = -1;
    }

    private void OnSlotSelected(int id, ProfileSlot slot)
    {
        selectedItemId = id;

        foreach (Transform t in slotParent)
        {
            var s = t.GetComponent<ProfileSlot>();
            if (s != null)
                s.SetSelected(s == slot);
        }
    }

    private void Apply()
    {
        if (selectedItemId < 0)
            return;

        Sprite newIcon = parentController?.EquipProfileIcon(selectedItemId);

        if (newIcon != null && currentTargetIcon != null)
            currentTargetIcon.SetIcon(newIcon);

        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            FirebaseDatabase.DefaultInstance
                .GetReference($"users/{user.UserId}/profile/profileIcon")
                .SetValueAsync(selectedItemId);
        }

        Close();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        currentTargetIcon = null;
        parentController = null;
        selectedItemId = -1;
    }
}