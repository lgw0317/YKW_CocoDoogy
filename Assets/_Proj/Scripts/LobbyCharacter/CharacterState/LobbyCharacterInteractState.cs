using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LCocoDoogyInteractState : LobbyCharacterBaseState
{
    private NavMeshAgent agent;

    public LCocoDoogyInteractState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm) : base(owner, fsm)
    {
        agent = owner.GetComponent<NavMeshAgent>();
    }

    public override void OnStateEnter()
    {
        if (!agent.enabled) agent.enabled = true;
        if (agent.enabled && !agent.isStopped) agent.isStopped = true;
        Debug.Log("코코두기 Interact 진입");
        owner.StartCoroutine(WaitAndFinish());
    }
    public override void OnStateUpdate() { }
    public override void OnStateExit()
    {
        owner.StopAllCoroutines();
        agent.isStopped = false;
    }

    private IEnumerator WaitAndFinish()
    {
        yield return new WaitForSeconds(3f);
        owner.EndInteract(0);
        fsm.ChangeState(owner.IdleState);
        yield break;
    }
}

public class LMasterInteractState : LobbyCharacterBaseState
{
    // 0 이면 일반 클릭시 상호작용
    // 1 이면 코코두기와 마스터 상호작용
    // 2 이면 코코두기와 동물들 상호작용
    private Transform[] waypoints;
    private LobbyCharacterAnim anim;
    private int type;

    public LMasterInteractState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, LobbyCharacterAnim anim, Transform[] waypoints, int type = 1) : base(owner, fsm)
    {
        this.waypoints = waypoints;
        this.type = type;
        this.anim = anim;
    }

    public override void OnStateEnter()
    {
        switch (type)
        {
            case 0:
                anim.InteractionAnim();
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }

    public override void OnStateExit() { }

    public override void OnStateUpdate() { }

}

public class LAnimalInteractState : LobbyCharacterBaseState
{
    // 0 이면 일반 클릭시 상호작용
    // 1 이면 코코두기와 마스터 상호작용
    // 2 이면 코코두기와 동물들 상호작용
    private Transform[] waypoints;
    private LobbyCharacterAnim anim;
    private int type;

    public LAnimalInteractState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, LobbyCharacterAnim anim, Transform[] waypoints, int type = 1) : base(owner, fsm)
    {
        this.waypoints = waypoints;
        this.type = type;
        this.anim = anim;
    }

    public override void OnStateEnter()
    {
        switch (type)
        {
            case 0:
                anim.InteractionAnim();
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }

    public override void OnStateExit() { }

    public override void OnStateUpdate() { }

}
