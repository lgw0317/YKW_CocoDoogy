using UnityEngine;

public class PlayerPushTrigger : MonoBehaviour
{
    //[SerializeField] float pushThreshold = 0.9f; // 얼마나 강하게 밀어야 푸시?
    //[SerializeField] Joystick joystick;  // 너의 조이스틱 컴포넌트 할당

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!other.CompareTag("Pushable")) return;

    //    PushableObjects pushable = other.GetComponent<PushableObjects>();
    //    if (pushable == null || pushable.IsMoving) return;

    //    Vector2 input = new Vector2(joystick.Horizontal, joystick.Vertical);
    //    if (input.magnitude < pushThreshold) return;

    //    Vector2Int dir = Get4Direction(input);
    //    pushable.Push(dir);
    //}

    //private Vector2Int Get4Direction(Vector2 input)
    //{
    //    if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
    //        return input.x > 0 ? Vector2Int.right : Vector2Int.left;
    //    else
    //        return input.y > 0 ? Vector2Int.up : Vector2Int.down;
    //}
}
