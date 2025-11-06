using UnityEngine;
using UnityEngine.AI;

public class LCocoDoogyStopState : LobbyCharacterBaseState
{
    private NavMeshAgent agent;

    public LCocoDoogyStopState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, NavMeshAgent agent) : base(owner, fsm)
    {
        this.agent = agent;
    }

    public override void OnStateEnter()
    {
        if (agent.enabled && !agent.isStopped) agent.isStopped = true;

    }

    public override void OnStateExit()
    {
        
    }

    public override void OnStateUpdate()
    {
        
    }
}

public class LMasterStopState : LobbyCharacterBaseState
{
    private NavMeshAgent agent;

    public LMasterStopState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, NavMeshAgent agent) : base(owner, fsm)
    {
        this.agent = agent;
    }

    public override void OnStateEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }
}

public class LAnimalStopState : LobbyCharacterBaseState
{
    private NavMeshAgent agent;

    public LAnimalStopState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, NavMeshAgent agent) : base(owner, fsm)
    {
        this.agent = agent;
    }

    public override void OnStateEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }
}
