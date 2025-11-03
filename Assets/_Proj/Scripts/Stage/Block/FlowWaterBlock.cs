using UnityEngine;

public class FlowWaterBlock : WaterBlock
{

    Vector3 velocity = Vector3.zero;
    
    //나무 블럭이 닿았을 시 물의 방향으로 한칸 이동하는 함수
    //이동하는 방향은 블록의 rotate의 y값을 이용
    //해당 방향으로 블록 한칸만큼 lerp이용해서 부드럽게 이동
    //OnTriggerStay로 변경해야 하나? 일단 Enter로 실험
    
    
    //TODO: 
    
    
    
    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log($"{name}:충돌 감지, 충돌 대상: {other.name}");
    //    if (other.CompareTag("WoodBox"))
    //    {
    //        Transform ot = other.GetComponent<Transform>();
    //        float flowTime = transform.GetChild(0).GetComponent<Water.Flow>().GetFlowTime();
    //        Vector3 dir = Vector3.zero;
    //        switch (transform.rotation.y)
    //        {
    //            case 0: dir = new Vector3(0, 0, 1); break;   // 아래로
    //            case 90: dir = new Vector3(-1, 0, 0); break;   // 오른쪽
    //            case 180: dir = new Vector3(0, 0, -1); break;  // 위로
    //            case -90: dir = new Vector3(1, 0, 0); break; // 왼쪽
    //        }

    //        Vector3 targetPos = transform.position;

    //        //현재 이동은 되나, targetPos를 Vector.zero로 설정해둬서 원점으로 이동함 수정할 예정
    //        ot.transform.position = Vector3.SmoothDamp(targetPos, targetPos + dir, ref velocity, flowTime);
    //    }
    //}
}
