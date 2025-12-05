using System;
using System.Collections.Generic;
using UnityEngine;

public struct ProfileRedDotState
{
    public bool hasNewIcon;   // 새 프로필 아이콘이 하나라도 있음?
}

public static class ProfileRedDotManager
{
    public static ProfileRedDotState Current { get; private set; }
    public static event Action<ProfileRedDotState> OnStateChanged;

    // 새 아이콘 추가 (퀘스트 보상 등에서 호출)
    public static void MarkNewIcon(int iconId)
    {
        if (UserData.Local == null || UserData.Local.master == null)
            return;

        var master = UserData.Local.master;

        if (master.newProfileIcons == null)
            master.newProfileIcons = new HashSet<int>();

        // 이미 있는 아이콘이면 무시
        if (!master.newProfileIcons.Add(iconId))
            return;

        // Firebase 저장
        master.Save();

        Recalculate();
    }

    // 아이콘을 “봤다/사용했다” → 새 아이콘 목록에서 제거
    public static void MarkSeenIcon(int iconId)
    {
        if (UserData.Local == null || UserData.Local.master == null)
            return;

        var master = UserData.Local.master;
        if (master.newProfileIcons == null)
            master.newProfileIcons = new HashSet<int>();

        if (master.newProfileIcons.Remove(iconId))
        {
            master.Save();
            Recalculate();
        }
    }

    // 특정 아이콘이 새 아이콘인지 여부
    public static bool IsNewIcon(int iconId)
    {
        if (UserData.Local == null || UserData.Local.master == null)
            return false;

        var master = UserData.Local.master;
        if (master.newProfileIcons == null)
            return false;

        return master.newProfileIcons.Contains(iconId);
    }

    public static void Recalculate()
    {
        ProfileRedDotState newState = default;

        if (UserData.Local != null && UserData.Local.master != null)
        {
            var set = UserData.Local.master.newProfileIcons;
            if (set != null && set.Count > 0)
                newState.hasNewIcon = true;
        }

        if (newState.hasNewIcon == Current.hasNewIcon)
            return;

        Current = newState;
        OnStateChanged?.Invoke(Current);
    }

    public static void ForceClear()
    {
        if (UserData.Local != null && UserData.Local.master != null)
        {
            UserData.Local.master.newProfileIcons?.Clear();
            UserData.Local.master.Save();
        }

        Current = default;
        OnStateChanged?.Invoke(Current);
    }
}
