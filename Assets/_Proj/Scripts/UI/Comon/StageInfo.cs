using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfo : MonoBehaviour
{
    public GameObject StagePrefab;

    public void ShowStages(string chapterId)
    {
        ClearStages();

        var chapterProvider = DataManager.Instance.Chapter;
        var stageProvider = DataManager.Instance.Stage;

        ChapterData chapter = chapterProvider.GetData(chapterId);
        if (chapter == null)
        {
            Debug.LogWarning($"[StageInfo] 챕터({chapterId}) 데이터를 찾을 수 없습니다.");
            return;
        }

        foreach (var stageId in chapter.chapter_staglist)
        {
            var stageData = stageProvider.GetData(stageId);
            if (stageData == null)
            {
                Debug.LogWarning($"[StageInfo] 스테이지({stageId}) 데이터를 찾을 수 없습니다.");
                continue;
            }

            CreateStageUI(stageData, stageProvider);
        }
    }

    void CreateStageUI(StageData data, StageProvider provider)
    {
        GameObject stageInstance = Instantiate(StagePrefab, transform);

        // 이미지
        Image[] imgs = stageInstance.GetComponentsInChildren<Image>(true);
        if (imgs.Length > 0)
            imgs[0].sprite = provider.GetIcon(data.stage_id);

        // 텍스트
        TextMeshProUGUI[] texts = stageInstance.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (texts.Length >= 2)
        {
            texts[0].text = data.stage_name;
            texts[1].text = data.stage_desc;
        }

        // 버튼
        Button stageButton = stageInstance.GetComponent<Button>();
        if (stageButton != null)
        {
            stageButton.onClick.AddListener(() =>
            {
                Debug.Log($"[StageInfo] 스테이지 선택됨: {data.stage_id}");
                // TODO: 스테이지 진입 로직 추가
            });
        }
    }

    void ClearStages()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}