using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class ShockDetectionTower : MonoBehaviour, ISignalSender
{
    [Header("Relay Settings")]
    public float relayRadius = 4f;
    public float relayDelay = 1f;
    public float towerCooldown = 10f;

    [Header("Occlusion")]
    public bool useOcclusion = true;
    public LayerMask occluderMask;
    public LayerMask towerLayer;

    void Awake()
    {
        // 범위 안의 문 자동 탐색
        var cols = Physics.OverlapSphere(transform.position, relayRadius, ~0);
        foreach (var c in cols)
        {
            var receiver = c.GetComponentInParent<ISignalReceiver>();
            if (receiver != null)
            {
                Receiver = receiver;
                Debug.Log($"[Tower] {name}: 자동으로 가까이에 있는 {receiver} 검색");
                break;
            }
        }
    }

    // --- ISignalSender ---
    public ISignalReceiver Receiver { get; set; }

    private bool isCooling = false;

    // 충격파 수신
    public void ReceiveShock(Vector3 origin)
    {
        if (isCooling)
        {
            Debug.Log($"[Tower] {name}: 쿨타임 중, 무시됨");
            return;
        }

        Debug.Log($"[Tower] {name}: 충격파 감지!");

        // ISignalSender 인터페이스 연결된 리시버에 신호 전송
        SendSignal();

        // 쿨타임 진입
        StartCoroutine(CooldownTimer());

        // 주변 탑에 릴레이(전이)
        StartCoroutine(RelayToNearbyTowers(origin));
    }

    // 쿨타임 코루틴
    private IEnumerator CooldownTimer()
    {
        isCooling = true;
        yield return new WaitForSeconds(towerCooldown);
        isCooling = false;
        Debug.Log($"[Tower] {name}: 쿨타임 종료");
    }

    // 충격파 릴레이
    private IEnumerator RelayToNearbyTowers(Vector3 origin)
    {
        yield return new WaitForSeconds(relayDelay);

        var cols = Physics.OverlapSphere(transform.position, relayRadius, towerLayer, QueryTriggerInteraction.Ignore);
        foreach (var c in cols)
        {
            if (c.transform == transform) continue;
            var tower = c.GetComponent<ShockDetectionTower>();
            if (!tower || tower.isCooling) continue;

            // 차폐 검사
            if (useOcclusion)
            {
                Vector3 p0 = transform.position + Vector3.up * 0.5f;
                Vector3 p1 = tower.transform.position + Vector3.up * 0.5f;
                Vector3 dir = p1 - p0; float dist = dir.magnitude;
                if (dist > 0.1f && Physics.Raycast(p0, dir.normalized, dist - 0.05f, occluderMask))
                    continue;
            }

            tower.ReceiveShock(transform.position);
        }
    }

    // --- ISignalSender ---
    public void SendSignal()
    {
        if (Receiver != null)
        {
            Receiver.ReceiveSignal();
            Debug.Log($"[Tower] {name}: 문에 신호 전송 완료");
        }
        else
        {
            Debug.Log($"[Tower] {name}: 연결된 수신기가 없음");
        }
    }

    public void ConnectReceiver(ISignalReceiver receiver)
    {
        Receiver = receiver;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, relayRadius);
    }
#endif
}
