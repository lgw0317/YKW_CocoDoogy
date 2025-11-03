using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfo : MonoBehaviour
{
    public GameObject StagePrefab;          // 버튼 프리팹
    public GameObject StageDetailPrefab;    // 상세창 프리팹
    public Transform stageParent;           // StagePrefab 붙일 위치
    public Transform detailParent;          // StageDetailPrefab 붙일 위치
    private List<GameObject> activeDetails = new List<GameObject>();

    public void ShowStages(string chapterId)
    {
        ClearStages();

        var chapterProvider = DataManager.Instance.Chapter;
        var stageProvider = DataManager.Instance.Stage;

        ChapterData chapter = chapterProvider.GetData(chapterId);
        if (chapter == null) return;

        foreach (var stageId in chapter.chapter_staglist)
        {
            StageData stageData = stageProvider.GetData(stageId);
            if (stageData == null) continue;

            CreateStageButton(stageData, stageProvider);
        }
    }

    void CreateStageButton(StageData data, StageProvider provider)
    {
        GameObject stageObj = Instantiate(StagePrefab, stageParent);

        // 버튼 텍스트
        TextMeshProUGUI[] texts = stageObj.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (texts.Length > 0)
            texts[0].text = data.stage_name;

        // 별 표시 (예시)
        Transform starGroup = stageObj.transform.Find("StarGroup");
        if (starGroup)
        {
            int stars = Random.Range(0, 4); // TODO: 실제 데이터
            for (int i = 0; i < 3; i++)
            {
                Image star = starGroup.GetChild(i).GetComponent<Image>();
                star.color = i < stars ? Color.yellow : Color.gray;
            }
        }

        // 클릭 이벤트
        Button btn = stageObj.GetComponent<Button>();
        btn.onClick.AddListener(() => ShowStageDetail(data, provider));
    }

    void ShowStageDetail(StageData data, StageProvider provider)
    {
        // 기존 상세창 제거
        foreach (var obj in activeDetails)
            Destroy(obj);
        activeDetails.Clear();

        // 새로운 상세창 Instantiate
        GameObject detailObj = Instantiate(StageDetailPrefab, detailParent);
        activeDetails.Add(detailObj);

        StageDetailInfo detail = detailObj.GetComponent<StageDetailInfo>();
        if (detail != null)
            detail.ShowDetail(data);
    }

    void ClearStages()
    {
        foreach (Transform child in stageParent)
            Destroy(child.gameObject);

        foreach (var obj in activeDetails)
            Destroy(obj);
        activeDetails.Clear();
    }
}