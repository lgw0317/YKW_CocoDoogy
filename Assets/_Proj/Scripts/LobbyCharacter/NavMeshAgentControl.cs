using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentControl
{
    // 필요한 기본 이동 로직들을 만들고 각 스크립트에 조립하는 형식이 좋겠지? 코루틴을 사용하는게 더 좋으니
    private readonly NavMeshAgent agent;
    private float moveSpeed; // �̵� �ӵ�
    private float angularSpeed; // �� �ӵ�
    private float acceleration; // ���ӵ�

    // �� ��
    private float moveRadius; // �̵� ����
    private float timer;
    private Transform transform;

    public NavMeshAgentControl(NavMeshAgent agent, float moveSpeed, float angularSpeed, float acceleration, float moveRadius, Transform transform)
    {
        this.agent = agent;
        this.moveSpeed = moveSpeed;
        this.angularSpeed = angularSpeed;
        this.acceleration = acceleration;
        this.moveRadius = moveRadius;
        this.transform = transform;
    }

    // 캐릭터 움직일 때 걷거나 뛰는 애니메이션 속도 값 리턴
    public float ValueOfMagnitude()
    {
        return agent.velocity.magnitude;
    }

    // 속도 변경 시 적용인데 아직 쓸 일이 없을 듯
    public void MoveValueChanged()
    {
        if (agent.speed != moveSpeed) agent.speed = moveSpeed;
        if (agent.angularSpeed != angularSpeed) agent.angularSpeed = angularSpeed;
        if (agent.acceleration != acceleration) agent.acceleration = acceleration;

    }

    // WaitUntil이나 WaitFS를 사용할 것이니 필요 없을 듯
    // public void WaitAndMove(Transform point, ref float waitTime)
    // {
    //     if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
    //     {

    //         timer += Time.deltaTime;
    //         if (timer >= waitTime)
    //         {
    //             MoveToTransPoint(point);
    //             timer = 0;
    //         }
    //     }
    // }

    /// <summary>
    /// Trans일 시 위치 이동
    /// </summary>
    /// <param name="point"></param>
    public void MoveToTransPoint(Transform point)
    {
        if (agent.isStopped) agent.isStopped = false;
        Vector3 pos = point.position;
        float speed = Random.Range(2f, 4f);
        agent.SetDestination(pos);
        agent.speed = speed;
        agent.acceleration = Random.Range(speed, 10f);
        //agent.stoppingDistance = Random.Range(0f, 0.5f);
    }
    /// <summary>
    /// Vector3일 시 위치 이동
    /// </summary>
    /// <param name="point"></param>
    public void MoveToVectorPoint(Vector3 point)
    {
        if (agent.isStopped) agent.isStopped = false;
        float speed = Random.Range(2f, 5f);
        agent.SetDestination(point);
        agent.speed = speed;
        agent.acceleration = Random.Range(speed, 10f);
        //agent.stoppingDistance = Random.Range(0f, 0.5f);
    }
    /// <summary>
    /// 마지막 포인트 지점일 시 느리게 이동(필요 없을 수 있음)
    /// </summary>
    /// <param name="point"></param>
    public void MoveToLastPoint(Transform point)
    {
        if (agent.isStopped) agent.isStopped = false;
        Vector3 lastPos = point.position;
        agent.SetDestination(lastPos);
        agent.speed = 2f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 0f;
    }
    /// <summary>
    /// 해당 포인트를 기준으로 랜덤 범위 이동
    /// </summary>
    /// <param name="point"></param>
    public void MoveToRandomTransPoint(Transform point)
    {
        if (agent.isStopped) agent.isStopped = false;
        Vector3 randomDir = point.position + Random.insideUnitSphere * moveRadius;
        randomDir.y = point.position.y;
        //randomDir += transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 0.3f, NavMesh.AllAreas))
        {
            //randomDir = hit.position;
            //agent.SetDestination(hit.position);
            MoveToVectorPoint(hit.position);
            //MoveToVectorPoint(randomDir);
        }
    }

    public void CocoMoveToRandomTransPoint(Transform point)
    {
        if (agent.isStopped) agent.isStopped = false;
        Vector3 randomDir = point.position + Random.insideUnitSphere * 1f;
        randomDir.y = point.position.y;
        //randomDir += transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            //randomDir = hit.position;
            MoveToVectorPoint(hit.position);
            //MoveToVectorPoint(randomDir);
        }
    }
    public void RestAgent()
    {
        if (agent.enabled == false) agent.enabled = true;
        if (agent.isStopped) agent.isStopped = false;
    }

    // 이 부분은 각 Behaviour 쪽 agent를 사용해서 관리하기로 함
    // public void AgentIsStop(bool which)
    // {
    //     if (which == true) agent.isStopped = true;
    //     else agent.isStopped = false;
    // }
    // public void EnableAgent(bool which)
    // {
    //     if (which == true) agent.enabled = true;
    //     else agent.enabled = false;
    // }

}
