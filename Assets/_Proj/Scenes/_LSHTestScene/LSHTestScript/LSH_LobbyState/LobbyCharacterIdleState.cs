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

    public LCocoDoogyIdleState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
    {
        agent = owner.GetComponent<NavMeshAgent>();
    }

    public override void OnStateEnter()
    {
        if (!agent.enabled) agent.enabled = true;
        var trans = owner.GetComponent<Transform>();
        agent.Warp(trans.position);
        Debug.Log("코코두기 Idle 진입");
        owner.StartCoroutine(WaitThenMove());
    }

    public override void OnStateExit()
    {
        owner.StopAllCoroutines();
    }

    public override void OnStateUpdate() { }
    
    private IEnumerator WaitThenMove()
    {
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        //fsm.ChangeState(new LCocoDoogyMoveState(owner, fsm, waypoints, currentIndex));
        fsm.ChangeState(owner.MoveState);
        //yield break;
    }
}

/// <summary>
/// 마스터
/// </summary>
public class LMasterIdleState : LobbyCharacterBaseState
{

    public LMasterIdleState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
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
        
    }
}

/// <summary>
/// 동물
/// </summary>
public class LAnimalIdleState : LobbyCharacterBaseState
{

    public LAnimalIdleState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
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
        
    }
}
