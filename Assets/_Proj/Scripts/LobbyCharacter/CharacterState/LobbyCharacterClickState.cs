using UnityEngine;
using UnityEngine.AI;

public class LCocoDoogyClickState : LobbyCharacterBaseState
{
    private readonly NavMeshAgent agent;
    private readonly LobbyCharacterAnim charAnim;
    public LCocoDoogyClickState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, LobbyCharacterAnim charAnim) : base(owner, fsm)
    {
        agent = owner.GetComponent<NavMeshAgent>();
        this.charAnim = charAnim;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Click 진입");
        if (agent.enabled && !agent.isStopped) agent.isStopped = true;
        var trans = owner.GetComponent<Transform>();
        charAnim.InteractionAnim();
        
        AudioEvents.Raise(SFXKey.CocodoogyFootstep, pooled: true, pos: trans.position);
    }

    public override void OnStateExit()
    {
        if (agent.enabled && agent.isStopped) agent.isStopped = false;
    }

    public override void OnStateUpdate() { }
}

public class LMasterClickState : LobbyCharacterBaseState
{
    public LMasterClickState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
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

public class LAnimalClickState : LobbyCharacterBaseState
{
    public LAnimalClickState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
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
