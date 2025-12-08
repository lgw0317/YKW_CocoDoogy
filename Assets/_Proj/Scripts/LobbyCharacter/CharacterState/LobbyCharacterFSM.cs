using UnityEngine;

public class LobbyCharacterFSM
{
    private LobbyCharacterBaseState currentState;
    public LobbyCharacterBaseState CurrentState => currentState;
    
    public LobbyCharacterFSM(LobbyCharacterBaseState initState)
    {
        currentState = initState;
        ChangeState(currentState);
    }

    public void ChangeState(LobbyCharacterBaseState nextState)
    {
        if (nextState == currentState) return;

        currentState?.OnStateExit();
        currentState = nextState;
        currentState?.OnStateEnter();
    }

    public void UpdateState()
    {
        currentState?.OnStateUpdate();
    }

}
