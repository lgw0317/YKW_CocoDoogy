using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 코코두기
/// </summary>
public class LCocoDoogyIdleState : LobbyCharacterBaseState
{
    private Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentIndex;

    public LCocoDoogyIdleState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, Transform[] waypoints, int currentIndex) : base(owner, fsm)
    {
        this.waypoints = waypoints;
        this.currentIndex = currentIndex;
        agent = owner.GetComponent<NavMeshAgent>();
    }

    public override void OnStateEnter()
    {
        if (!agent.enabled) agent.enabled = true;
        if (agent.enabled && agent.isStopped) agent.isStopped = false;
        Debug.Log("코코두기 Idle 진입");
        owner.StartCoroutine(WaitThenMove());
    }

    public override void OnStateExit() { }

    public override void OnStateUpdate() { }
    
    private IEnumerator WaitThenMove()
    {
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        //fsm.ChangeState(new LCocoDoogyMoveState(owner, fsm, waypoints, currentIndex));
        fsm.ChangeState(new LCocoDoogyMoveState(owner, fsm, agent, waypoints, currentIndex));
        //yield break;
    }
}

/// <summary>
/// 마스터
/// </summary>
public class LMasterIdleState : LobbyCharacterBaseState
{
    private Transform[] waypoints;

    public LMasterIdleState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, Transform[] waypoints) : base(owner, fsm)
    {
        this.waypoints = waypoints;
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
        
    }
}

/// <summary>
/// 동물
/// </summary>
public class LAnimalIdleState : LobbyCharacterBaseState
{
    private Transform[] waypoints;

    public LAnimalIdleState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, Transform[] waypoints) : base(owner, fsm)
    {
        this.waypoints = waypoints;
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
        
    }
}
