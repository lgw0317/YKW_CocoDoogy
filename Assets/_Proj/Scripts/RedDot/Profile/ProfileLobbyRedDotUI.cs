using UnityEngine;

public class ProfileLobbyRedDotUI : MonoBehaviour
{
    [SerializeField] private GameObject redDot;   // 버튼 위 빨간점 오브젝트

    private void OnEnable()
    {
        ProfileRedDotManager.OnStateChanged += Apply;
        Apply(ProfileRedDotManager.Current);
    }

    private void OnDisable()
    {
        ProfileRedDotManager.OnStateChanged -= Apply;
    }

    private void Apply(ProfileRedDotState state)
    {
        if (redDot)
            redDot.SetActive(state.hasNewIcon);
    }
}
