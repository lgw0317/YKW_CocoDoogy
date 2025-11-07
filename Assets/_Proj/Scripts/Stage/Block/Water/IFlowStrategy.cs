using UnityEngine;

public interface IFlowStrategy
{
    // 물의 흐름에 따라 객체를 이동시키는 로직을 수행
    void ExecuteFlow(PushableObjects target, Vector2Int flowDir);
}