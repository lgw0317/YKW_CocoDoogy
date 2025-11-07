using UnityEngine;

public class FlowWaterStrategy : IFlowStrategy
{
    // 이동 코루틴은 PushableObjects 내부에서 처리
    public void ExecuteFlow(PushableObjects target, Vector2Int flowDir)
    {
        target.ImmediatePush(flowDir);
    }
}