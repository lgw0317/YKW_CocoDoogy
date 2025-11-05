using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지정한 패널들 중 하나라도 활성화(activeInHierarchy)면 QuarterView 카메라 회전을 잠근다.
/// (툴바/커런시처럼 항상 열려 있어야 하는 건 리스트에 넣지 마세요)
/// </summary>
[DisallowMultipleComponent]
public class OrbitBlockOnPanels : MonoBehaviour
{
    [SerializeField] private List<GameObject> panelsToBlock = new(); // 6개만 등록

    private bool pushed;

    private void LateUpdate()
    {
        bool anyOpen = false;
        for (int i = 0; i < panelsToBlock.Count; i++)
        {
            var go = panelsToBlock[i];
            if (go && go.activeInHierarchy) { anyOpen = true; break; }
        }

        if (anyOpen && !pushed)
        {
            QuarterView.PushUIOrbitBlock();
            pushed = true;
        }
        else if (!anyOpen && pushed)
        {
            QuarterView.PopUIOrbitBlock();
            pushed = false;
        }
    }

    private void OnDisable()
    {
        if (pushed)
        {
            QuarterView.PopUIOrbitBlock();
            pushed = false;
        }
    }
}
