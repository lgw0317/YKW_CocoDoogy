using System.Collections;
using UnityEngine;

public class AnimalBehaviour : BaseLobbyCharacterBehaviour
{
    [SerializeField] float decoDetectRadius = 20f; // 데코 오브젝트 탐색 범위
    private Transform targetDeco;

    protected override void InitStates()
    {
        throw new System.NotImplementedException();
    }

    
    protected override void Awake()
    {
        base.Awake();
        agent.avoidancePriority = 50;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        targetDeco = null;
    }

    // private IEnumerator Move()
    // {
    //     isMoving = true;
    //     while (isMoving)
    //     {
    //         FindNearestDeco();
    //         if (targetDeco != null)
    //         {
    //             charAgent.MoveToRandomTransPoint(targetDeco);
    //         }
    //         else
    //         {
    //             charAgent.MoveToRandomTransPoint(transform);
    //         }

    //         if (!agent.hasPath) charAgent.MoveToRandomTransPoint(transform);
    //         yield return waitU;
    //     }
    // }

    private void FindNearestDeco()
    {
        // 나중에 로비 안에 활성화 된 데코 리스트가 있으면 대체
        GameObject[] decos = GameObject.FindGameObjectsWithTag("Decoration");
        float nearestDist = float.MaxValue;
        Transform nearest = null;

        foreach (GameObject deco in decos)
        {
            float dist = Vector3.Distance(transform.position, deco.transform.position);
            if (dist < decoDetectRadius && dist < nearestDist)
            {
                nearestDist = dist;
                nearest = deco.transform;
            }
        }
        targetDeco = nearest;
    }

    // 인터페이스 영역
    public override void OnCocoAnimalEmotion()
    {
        if (!agent.isStopped) agent.isStopped = true;

    }

    public override void OnCocoMasterEmotion()
    {
        
    }

    public override void OnLobbyEndDrag(Vector3 position)
    {
        base.OnLobbyEndDrag(position);
    }
    public override void OnLobbyClick()
    {
        base.OnLobbyClick();
        charAnim.InteractionAnim();
        AudioEvents.Raise(SFXKey.CocodoogyFootstep, pooled: true, pos: transform.position); // 각 동물 소리로
    }
    public override void InNormal()
    {
        base.InNormal();
    }

    

    // public override void ExitScene()
    // {

    // }
}
