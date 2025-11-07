using System;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    private string treasureId;
    private bool isCollected = false;

    public void Init(string id)
    {
        treasureId = id;
        Debug.Log($"[Treasure] Init 완료 → ID: {treasureId}");

    }
    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;
            StageUIManager.Instance.TreasurePanel.SetActive(true);
            StageUIManager.Instance.OptionOpenButton.gameObject.SetActive(false);

            var data = DataManager.Instance.Treasure.GetData(treasureId);

            switch(data.treasureType)
            {
                case TreasureType.deco:
                    var deco = DataManager.Instance.Deco.GetData(data.reward_id);
                    StageUIManager.Instance.TreasureName.text = deco.deco_name;
                    StageUIManager.Instance.TreasureDesc.text = deco.deco_desc;
                    TreasureUI(data);
                    break;
                case TreasureType.costume:
                    var costume = DataManager.Instance.Costume.GetData(data.reward_id);
                    StageUIManager.Instance.TreasureName.text = costume.costume_name;
                    StageUIManager.Instance.TreasureDesc.text = costume.costume_desc;
                    TreasureUI(data);
                    break;
                case TreasureType.artifact:
                    var artifact = DataManager.Instance.Artifact.GetData(data.reward_id);
                    StageUIManager.Instance.TreasureName.text = artifact.artifact_name;
                    StageUIManager.Instance.TreasureDesc.text = artifact.artifact_name;
                    TreasureUI(data);
                    break;
                case TreasureType.coin:
                case TreasureType.cap:
                    StageUIManager.Instance.TreasureName.text = "보물이지롱";
                    StageUIManager.Instance.TreasureDesc.text = "사실아니지롱";
                    TreasureUI(data);
                    break;
            }


            // 플레이어 이동 막기
            other.GetComponent<PlayerMovement>().enabled = false;

            // 확인 버튼 클릭 시 호출되도록 이벤트 등록
            StageUIManager.Instance.OnTreasureConfirm = () => OnQuitAction(() =>
            {
                // 획득 처리
                StageUIManager.Instance.stageManager.OnTreasureCollected(treasureId);

                // UI 닫기
                StageUIManager.Instance.TreasurePanel.SetActive(false);
                StageUIManager.Instance.OptionOpenButton.gameObject.SetActive(true);

                // 플레이어 이동 복원
                other.GetComponent<PlayerMovement>().enabled = true;
            });
        }
    }

    private static void TreasureUI(TreasureData data)
    {
        StageUIManager.Instance.TreasureImage.sprite = DataManager.Instance.Deco.GetIcon(data.reward_id);
        StageUIManager.Instance.TreasureType.text = data.treasureType.ToString();
        StageUIManager.Instance.TreasureCount.text = data.count.ToString();
        StageUIManager.Instance.CocoDoogyDesc.text = data.coco_coment;
    }

    public void OnQuitAction(Action action)
    {
        action?.Invoke();
    }
}
