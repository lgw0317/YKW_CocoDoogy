using UnityEngine;

public class FlowWaterStrategy : IFlowStrategy
{
    // 이동 코루틴은 PushableObjects 내부에서 처리
    public void ExecuteFlow(PushableObjects target, Vector3 flowDir)
    {
        // 이미 이동 중이거나 낙하 중이면 흐름을 무시
        if (target.IsMoving || target.IsFalling)
            return;

        // 흐름 방향을 XZ 평면 Vector2Int으로 변환
        Vector2Int dir2D = new Vector2Int(
            Mathf.RoundToInt(flowDir.x),
            Mathf.RoundToInt(flowDir.z)
        );

        // ImmediatePush를 호출하면 PushableObjects 내부에서 BlockingMask 검사 후 TryPush 시작
        target.ImmediatePush(dir2D);
    }
}