using UnityEngine;

public abstract class LobbyCharacterBaseState
{
    protected BaseLobbyCharacterBehaviour owner;
    protected LobbyCharacterFSM fsm;

    protected LobbyCharacterBaseState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm)
    {
        this.owner = owner;
        this.fsm = fsm;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
