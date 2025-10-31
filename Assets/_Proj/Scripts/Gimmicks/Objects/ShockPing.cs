using UnityEngine;
using System.Threading;

public class ShockPing : MonoBehaviour
{
    [Tooltip("Same Object's Shockwave")]
    public Shockwave shockwave;

    [Tooltip("Tower Layer")]
    public LayerMask towerLayer;
    public bool useOcclusion = false;
    [Tooltip("벽/지형 레이어")]
    public LayerMask occludeLayer;

    private static long _seed = 1;
    static long NewToken() => Interlocked.Increment(ref _seed);

    void Awake()
    {
        if (!shockwave) shockwave = GetComponent<Shockwave>();
    }

    // Shockwave 원점 기준 반경 내 탑들에 신호 보냄
    public void PingTowers(Vector3 origin)
    {
        if (!shockwave) shockwave = GetComponent<Shockwave>();
        if (!shockwave) return;

        float rW = Mathf.Max(0.0001f, shockwave.radiusShock) * Mathf.Max(0.0001f, shockwave.tileHeight);
        var hits = Physics.OverlapSphere(origin, rW, towerLayer, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return;

        int sentCnt = 0;
        foreach (var h in hits)
        {
            var tower = h.GetComponentInParent<ShockDetectionTower>();
            if (!tower) continue;

            if (useOcclusion)
            {
                Vector3 p0 = origin + Vector3.up * 0.1f;
                Vector3 p1 = tower.transform.position + Vector3.up * 0.5f;
                Vector3 dir = p1 - p0;
                float dist = dir.magnitude;
                if (dist > 0.01f &&
                    Physics.Raycast(p0, dir.normalized, dist - 0.05f, occludeLayer, QueryTriggerInteraction.Ignore))
                    continue;
            }

            tower.ReceiveShock(origin); // 💥 새 시그니처
            sentCnt++;
        }

        Debug.Log($"[Ping] 감지탑 핑 전송 완료 ({sentCnt}개 감지)", this);
    }
}
