using UnityEngine;

public class LobbyCharacterFSM
{
    public LobbyCharacterFSM(LobbyCharacterBaseState initState)
    {
        currentState = initState;
        ChangeState(currentState);
    }

    private LobbyCharacterBaseState currentState;

    public void ChangeState(LobbyCharacterBaseState nextState)
    {
        if (nextState == currentState) return;

        if (currentState != null) currentState?.OnStateExit(); // 다음 state 가기 전에 저장 용도로?

        currentState = nextState;
        currentState?.OnStateEnter();
    }

    public void UpdateState()
    {
        if (currentState != null) currentState?.OnStateUpdate();
    }
}
