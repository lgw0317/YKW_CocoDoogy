using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Joystick joystick;
    public Rigidbody rb;

    public float moveSpeed = 5.0f;
    public float rotateLerp = 12f;

    // 캐릭터의 현재 이동 방향을 월드 좌표계에 맞게 변환하기 위해 필요
    private Transform camTr;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camTr = Camera.main != null ? Camera.main.transform : null;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (joystick == null) return;
        if (camTr == null) camTr = Camera.main?.transform;

        Vector2 input = new Vector2(joystick.InputDir.x, joystick.InputDir.z);

        if(input.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        Vector3 fwd = camTr ? camTr.forward : Vector3.forward;
        Vector3 right = camTr ? camTr.right : Vector3.right;
        fwd.y = 0;
        right.y = 0;
        fwd.Normalize();
        right.Normalize();

        // 최종 월드 이동 방향 계산
        // 조이스틱의 X를 카메라의 Right 방향에, Z를 카메라의 Forward 방향에 적용
        Vector3 moveDir = (right * input.x) + (fwd * input.y);
        if (moveDir.sqrMagnitude > 0.1f) moveDir.Normalize();

        //Vector3 vel = moveDir * moveSpeed;
        //vel.y = rb.linearVelocity.y;
        //rb.linearVelocity = vel;

        // 그리드/타일 기반에서는 MovePosition이 적합
        Vector3 nextPos = rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(nextPos);

        Quaternion targetRot = Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.z), Vector3.up);
        Quaternion smoothRot = Quaternion.Slerp(rb.rotation, targetRot, rotateLerp * Time.fixedDeltaTime);
        rb.MoveRotation(smoothRot);
    }
}