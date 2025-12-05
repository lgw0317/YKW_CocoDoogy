using UnityEngine;

public class PlayerTransparentCollider : MonoBehaviour, IMoveStrategy
{

    public LayerMask transparentMask;
    public (Vector3, Vector3) Execute(Vector3 moveDir, Rigidbody rb, PlayerMovement player)
    {
        Vector3 finalDir = moveDir;

        ////박스캐스트 방식
        //Vector3 center = rb.position + Vector3.up * .5f + rb.transform.forward * .25f;
        //Vector3 halfExt = Vector3.one * .25f;
        //Quaternion rotation = rb.rotation;
        //if (Physics.BoxCast(center, halfExt, moveDir, rotation, .1f, transparentMask))
        //{
        //    Debug.Log($"플레이어 이동전략(투명벽 전략): 박스캐스트로 TransparentWall 검출됨.");
        //    finalDir = Vector3.ClampMagnitude(finalDir, .005f);
        //}

        //레이캐스트 방식
        Ray ray = new(rb.position + Vector3.up * .5f, moveDir);
        if (Physics.Raycast(ray, .5f, transparentMask))
        {
            Debug.Log($"플레이어 이동전략(투명벽 전략): 레이캐스트로 TransparentWall 검출됨.");
            finalDir = Vector3.ClampMagnitude(finalDir, .2f);
        }

        return (finalDir, Vector3.zero);
    }


    void Reset()
    {
        transparentMask = LayerMask.GetMask("TransparentWall");
    }

}
