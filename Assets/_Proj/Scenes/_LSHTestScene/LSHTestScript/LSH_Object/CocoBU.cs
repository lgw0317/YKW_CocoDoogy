// using System.Collections;
// using Unity.VisualScripting;
// using UnityEngine;

// // 코코두기가 안드로이드와 특정 범위 내에 있거나 랜덤 값으로
// // 뽑힌 동물들에게 다가갔을때 로비 매니저에게 이벤트 호출, 로비매니저는
// // 상호작용 실행.

// public class CocoBU : BaseLobbyCharacterBehaviour
// {
//     private LCocoDoogyIdleState idle;
//     private LCocoDoogyInteractState interact;
//     private LCocoDoogyMoveState move;
//     private LCocoDoogyStopState stop;
//     private Transform currentDestination;
//     private int currentWaypointIndex; // 웨이포인트 이동 인덱스

//     public override LobbyCharacterBaseState InitialState()
//     {
//         //return new LCocoDoogyIdleState(this, fsm, waypoints, currentWaypointIndex);
//         return idle;
//     }    public override LobbyCharacterBaseState InteractState()
//     {
//         return interact;
//     }
//     public override LobbyCharacterBaseState MoveState()
//     {
//         return move;
//     }
//     public override LobbyCharacterBaseState StopState()
//     {
//         return stop;
//     }

//     protected override void Awake()
//     {
//         base.Awake();
//         agent.avoidancePriority = 99;
//         idle = new LCocoDoogyIdleState(this, fsm, waypoints, currentWaypointIndex);
//         interact = new LCocoDoogyInteractState(this, fsm, waypoints);
//         move = new LCocoDoogyMoveState(this, fsm, waypoints, currentWaypointIndex);
//         stop = new LCocoDoogyStopState(this, fsm, agent);
//         for (int i = 0; i < InLobbyManager.Instance.cocoWaypoints.Length; i++)
//         {
//             waypoints[i] = InLobbyManager.Instance.cocoWaypoints[i];
//         }
//     }

//     protected override void OnEnable()
//     {
//         base.OnEnable();
//         currentWaypointIndex = 0;

//     }

//     protected override void Start()
//     {
//         base.Start();
//     }

//     // 가자 멍뭉아
//     // private IEnumerator LetsGoCoco(Transform[] waypoints)
//     // {
//     //     Debug.Log($"시작 인덱스 값 : {currentWaypointIndex}");
//     //     isMoving = true;
//     //     while (isMoving)
//     //     {
//     //         if (currentWaypointIndex == waypoints.Length - 1)
//     //         {
//     //             //charAgent.MoveToTransPoint(waypoints[currentWaypointIndex]);
//     //             charAgent.CocoMoveToRandomTransPoint(waypoints[currentWaypointIndex]);
//     //             currentWaypointIndex = 0;
//     //         }
//     //         if (currentWaypointIndex == 0)
//     //         {
//     //             charAgent.MoveToLastPoint(waypoints[currentWaypointIndex]);
//     //             yield return waitFS;
//     //         }
//     //         currentWaypointIndex++;
//     //         Debug.Log($"현재 인덱스 값 : {currentWaypointIndex}");
//     //         //charAgent.MoveToTransPoint(waypoints[currentWaypointIndex]);
//     //         charAgent.CocoMoveToRandomTransPoint(waypoints[currentWaypointIndex]);
//     //         yield return waitU;

//     //     }
//     // }

//     protected override void HandleIdle()
//     {
        
//     }

//     protected override void HandleMove()
//     {
//         agent.enabled = true;
//         if (agent.enabled && agent.isStopped) agent.isStopped = false;
//         if (!agent.hasPath)
//         {
//             if (currentWaypointIndex < InLobbyManager.Instance.cocoWaypoints.Length - 1)
//             {
//                 //charAgent.CocoMoveToRandomTransPoint(InLobbyManager.Instance.cocoWaypoints[currentWaypointIndex]);
//                 Vector3 pos = InLobbyManager.Instance.cocoWaypoints[currentWaypointIndex].position;
//                 agent.SetDestination(pos);
//             }
//             else if (currentWaypointIndex == InLobbyManager.Instance.cocoWaypoints.Length)
//             {
//                 ChangeState(LobbyCharacterState.Idle);
//             }
            
//         }

//         // 거의 도착한 경우
//         if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
//         {
//             if (agent.velocity.sqrMagnitude < 0.01f)
//             {
//                 if (currentWaypointIndex < InLobbyManager.Instance.cocoWaypoints.Length - 1)
//                 {
//                     currentWaypointIndex++;
//                 }
//             }
//         }

//         // 이동 중 멈춤 감지
//         if (!agent.isStopped && agent.velocity.sqrMagnitude < 0.01f)
//         {
//             stuckTimeA += Time.deltaTime;
//             if (stuckTimeA > stuckTimeB)
//             {
//                 ChangeState(LobbyCharacterState.Stuck);
//             }
//         }
//         else
//         {
//             stuckTimeA = 0f;
//         }
//     }

//     protected override void HandleStuck()
//     {
//         ChangeState(LobbyCharacterState.Recovering);
//         StartCoroutine(RecoverAndMove());
//     }

//     protected override void HandleInteraction()
//     {
//         if (agent.enabled && !agent.isStopped) agent.isStopped = true;
//     }

//     protected override void HandleAnimation()
//     {
//         if (agent.enabled && !agent.isStopped) agent.isStopped = true;
//     }

//     protected override void HandleCocoOther()
//     {
//         if (agent.enabled && !agent.isStopped) agent.isStopped = true;
//     }

//     protected override void ChangeState(LobbyCharacterState newState)
//     {
//         if (currentState == newState) return;

//         currentState = newState;
//         Debug.Log($"State 변경 : {currentState}");

//         if (newState == LobbyCharacterState.Move)
//         {
//             currentDestination = InLobbyManager.Instance.cocoWaypoints[currentWaypointIndex];
//             if (agent.enabled && agent.isStopped) agent.isStopped = false;
//             //charAgent.CocoMoveToRandomTransPoint(currentDestination);
//             agent.SetDestination(currentDestination.position);
//         }
//         else if (newState == LobbyCharacterState.Animation || newState == LobbyCharacterState.Dragging || newState == LobbyCharacterState.CocoOther)
//         {
//             if (agent.enabled && !agent.isStopped) agent.isStopped = true;
//         }
//     }

//     IEnumerator RecoverAndMove()
//     {
//         yield return new WaitForSeconds(3f);

//         if (agent.enabled && agent.isStopped) agent.isStopped = false;
//         Vector3 pos = InLobbyManager.Instance.cocoWaypoints[currentWaypointIndex].position;
//         agent.SetDestination(pos);
//         //charAgent.CocoMoveToRandomTransPoint(InLobbyManager.Instance.cocoWaypoints[currentWaypointIndex]);
//         stuckTimeB = 0f;

//         ChangeState(LobbyCharacterState.Move);
//     }
//     // 드래그나 편집모드 성공적으로 끝날 시 본인 위치 기준 가장 가까운 포인트 찾기 
//     private int GetClosestWaypointIndex()
//     {
//         float minDistance = float.MaxValue;
//         int closestIndex = 0;

//         for (int i = 0; i < InLobbyManager.Instance.cocoWaypoints.Length; i++)
//         {
//             float distance = Vector3.Distance(transform.position, InLobbyManager.Instance.cocoWaypoints[i].position);
//             if (distance < minDistance)
//             {
//                 minDistance = distance;
//                 closestIndex = i;
//             }
//         }
//         return closestIndex;
//     }

//     // 인터페이스 영역
//     public override void OnCocoAnimalEmotion()
//     {
        
//     }

//     public override void OnCocoMasterEmotion()
//     {
        
//     }

//     public override void OnLobbyEndDrag(Vector3 position)
//     {
//         currentWaypointIndex = GetClosestWaypointIndex();
//         base.OnLobbyEndDrag(position);
//         //StartCoroutine(LetsGoCoco(InLobbyManager.Instance.cocoWaypoints));
//     }
//     public override void OnLobbyClick()
//     {
//         base.OnLobbyClick();
//         charAnim.InteractionAnim();
//         AudioEvents.Raise(SFXKey.CocodoogyFootstep, pooled: true, pos: transform.position);
//     }
//     public override void InNormal()
//     {
//         currentWaypointIndex = GetClosestWaypointIndex();
//         base.InNormal();
//         ChangeState(LobbyCharacterState.Move);
//         //StartCoroutine(LetsGoCoco(InLobbyManager.Instance.cocoWaypoints));
//     }
//     public override void StartScene()
//     {
//         Debug.Log("시작");
//         Debug.Log($"지금 인덱스 값: {currentWaypointIndex}");
//         ChangeState(LobbyCharacterState.Move);
//     }

    


//     // public override void ExitScene()
//     // {

//     // }

// }