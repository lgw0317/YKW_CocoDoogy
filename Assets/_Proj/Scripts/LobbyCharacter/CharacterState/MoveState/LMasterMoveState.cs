using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LMasterMoveState : LobbyCharacterBaseState
{
    private readonly NavMeshAgent agent;
    private readonly NavMeshAgentControl charAgent;
    private LMasterRouteManager route;
    private Transform startPoint;
    private WaitUntil waitU;

    public LMasterMoveState(BaseLobbyCharacterBehaviour owner, LobbyCharacterFSM fsm, NavMeshAgentControl charAgent) : base(owner, fsm)
    {
        agent = owner.GetComponent<NavMeshAgent>();
        this.charAgent = charAgent;
        route = new LMasterRouteManager(owner.transform);
        waitU = new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= 0.5f);
        startPoint = owner.transform; // 로비매니저가 0번째 웨이포인트로 소환하니 상관없겠지?
    }

    public override void OnStateEnter()
    {
        if (!agent.enabled) agent.enabled = true;
        if (agent.enabled && agent.isStopped) agent.isStopped = false;

        Debug.Log($"{owner.gameObject.name} Move 진입");
        //owner.EndRoutine();
        owner.StartCoroutine(MoveRoutine());
    }
    public override void OnStateUpdate()
    {
        if (owner.gameObject.IsDestroyed()) owner.StopAllCoroutines();
        
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            //fsm.ChangeState(new LCocoDoogyInteractState(owner, fsm, waypoints));
            //fsm.ChangeState();
        }
        // 이동 중 멈춤 감지
        if (!agent.isStopped && agent.velocity.sqrMagnitude < 0.01f)
        {
            owner.StuckTimeA += Time.deltaTime;
            if (owner.StuckTimeA > owner.StuckTimeB)
            {
                fsm.ChangeState(owner.StuckState);
            }
        }
        else
        {
            owner.StuckTimeA = 0f;
        }
    }
    public override void OnStateExit()
    {
        owner.StopAllCoroutines();
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            route.RefreshDecoList();

            Transform next = route.GetNextDeco();
            if (next == null)
            {
                Debug.Log("(decoList == null || decoList.Count == 0 || hasComplete == true, 스타트 지점으로 감");
                agent.SetDestination(startPoint.position);
                yield return waitU;

                fsm.ChangeState(owner.IdleState);
                yield break;
            }

            Vector3 randDir = Random.insideUnitSphere * 2f;
            randDir.y = owner.YValue;
            Vector3 moveDir = randDir + next.position;
            agent.SetDestination(moveDir);
            yield return waitU;

            if (route.hasComplete)
            {
                // 루틴 끝
                agent.SetDestination(startPoint.position);
                yield return waitU;

                fsm.ChangeState(owner.EditState);
                // 로비 매니저에게 나 끝났어요 호출 하면 로비매니저가 SetActive false 처리

                yield break;
            }

        }
    }
    // 로비매니저에 있는 데코리스트들을 받아서 자신에게 가까운 순으로 정렬한 뒤 움직이기
    // 본인이 가지고 있는 리스트(decoList)와 로비매니저에서 리스트를 비교한 다음 본인이 가지고 있는 리스트에 로비매니저에서 새로운 이름이 들어가 있다면 추가하고 다시 정렬한 다음 전에 이동 했었던 오브젝트에게는 안가고 새로 추가된 녀석에게만 감. 만약, 본인이 가지고 있는 리스트에 오브젝트 이름 중 로비매니저가 가지고 있는 리스트에 없다면 그 오브젝트는 리스트에서 빠짐. 나중에 추가 된다면 해당 오브젝트로 가야함. 즉 로비매니저가 가지고 있는 리스트에 추가된 녀석이면 => 정렬하고 이동, 본인에게만 가지고 있는 리스트 오브젝트라면 삭제.
}
