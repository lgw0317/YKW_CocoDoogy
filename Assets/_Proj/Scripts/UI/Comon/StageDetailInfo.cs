using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageDetailInfo : MonoBehaviour
{
    public Image stageImage;
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI stageDesc;
    public Transform rewardGroup; // 보상 UI 부모

    public void ShowDetail(string id)
    {
        var data = DataManager.Instance.Stage.GetData(id);
        // 이미지
        if (stageImage != null)
            stageImage.sprite = DataManager.Instance.Stage.GetIcon(id);

        // 텍스트
        if (stageName != null) stageName.text = data.stage_name;
        if (stageDesc != null) stageDesc.text = data.stage_desc;

        // TODO: 보상 표시 (treasure_01_id ~ 03_id)
        // rewardGroup 아래 동적 생성 가능
    }
}