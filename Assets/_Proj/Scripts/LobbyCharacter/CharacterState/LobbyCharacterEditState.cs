using UnityEngine;
using UnityEngine.AI;

public class LCocoDoogyEditState : LobbyCharacterBaseState
{
    
    public LCocoDoogyEditState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm) { }

    public override void OnStateEnter()
    {
        Debug.Log("Edit 진입");
        var agent = owner.GetComponent<NavMeshAgent>();
        var anim = owner.GetComponent<Animator>();
        if (agent.enabled && !agent.isStopped) agent.isStopped = true;
        if (agent.enabled) agent.enabled = false;
        anim.Play("Idle_A");
    }

    public override void OnStateExit() { }

    public override void OnStateUpdate() { }
}

public class LMasterEditState : LobbyCharacterBaseState
{
    public LMasterEditState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
    {
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

public class LAnimalEditState : LobbyCharacterBaseState
{
    public LAnimalEditState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
    {
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
