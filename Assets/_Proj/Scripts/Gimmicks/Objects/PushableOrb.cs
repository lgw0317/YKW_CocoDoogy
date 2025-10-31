using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 10/30 TODO : Shockwave 사용해서 버팔로와 같이 충격파 발생시킬 수 있도록 수정.
// 버팔로와 다르게 낙하 시점에 충격파를 발생시켜야 함. -> 감지탑에 충격파 발생 여부 전달
// 물체에 의해 막힐 수 있음.
// 낙하하고 있다는 걸 어떻게 알지? 버팔로 뿐 아니라 그냥 절벽에서 떨어지더라도 충격파 발생됨.
// 충격파 발생 shockwave.Fire();

[RequireComponent(typeof(Shockwave))]
public class PushableOrb : PushableObjects
{
    SphereCollider sph;

    [SerializeField] private Shockwave shockwave;
    [SerializeField] private ShockPing shockPing;

    [Tooltip("Orb 충격파 쿨타임")]
    public float orbCoolTime = 6f;
    private float lastShockwaveTime = -float.MaxValue;
    private static Dictionary<int, float> floorCooldowns = new();

    [Header("Orb Fall Detection")]
    public float probeUp = 0.1f;
    public float probeDown = 1.5f;
    private bool wasGrounded;


    protected override void Awake()
    {
        base.Awake();
        sph = GetComponent<SphereCollider>();
        allowSlope = true;
        shockwave = GetComponent<Shockwave>();
        shockPing = GetComponent<ShockPing>();
        wasGrounded = true;
    }

    void FixedUpdate()
    {
        if (isMoving || isFalling)
        {
            // 이동/낙하 중일 때 wasGrounded를 false로 유지 (공중 상태로 간주)
            wasGrounded = false;
            return;
        }

        // Raycast를 사용하여 현재 땅에 닿아있는지 확인
        // tileSize를 곱하여 월드 단위 길이로 변환
        bool grounded = Physics.Raycast(
            origin: transform.position + Vector3.up * probeUp * tileSize,
            direction: Vector3.down,
            maxDistance: probeDown * tileSize,
            layerMask: groundMask,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );

        // 이전에는 공중이었는데, 지금은 지상에 닿은 경우 = 착지 완료
        if (!wasGrounded && grounded) // TODO : 여기 조건이 이상한 것 같은데...어떤 조건으로 설정해줘야 wavelift가 중복되지 않을까...
        {
            Debug.Log($"[Orb] 충격파 발생 {name}", this);
            TryFireShockwave(); // 충격파 발생 시도
        }
        wasGrounded = grounded;
    }



    protected override bool CheckBlocking(Vector3 target)
    {
        float r = sph.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) - 0.005f;
        Vector3 center = new(target.x, target.y + r, target.z);

        if (Physics.CheckSphere(center, r, blockingMask, QueryTriggerInteraction.Ignore))
            return true;

        var hits = Physics.OverlapSphere(center, r, ~throughLayer, QueryTriggerInteraction.Ignore);
        foreach (var c in hits)
        {
            // if ((groundMask.value & (1 << c.gameObject.layer)) != 0) continue; // 필요 시 바닥 제외
            if (c.transform.IsChildOf(transform)) continue;
            return true;
        }
        return false;
    }

    protected override bool IsImmuneToWaveLift()
    {
        return Time.time < lastShockwaveTime + orbCoolTime;
    }

    void TryFireShockwave()
    {
        if (shockwave == null) return;
        int yFloor = Mathf.RoundToInt(transform.position.y / tileSize);

        float now = Time.time;

        if (floorCooldowns.TryGetValue(yFloor, out var lastTime) && now - lastTime < orbCoolTime) return;

        if (now - lastShockwaveTime < orbCoolTime) return;

        floorCooldowns[yFloor] = now;
        lastShockwaveTime = now;

        Debug.Log($"[Orb] {yFloor}층 충격파 발생", this);
        WaveLift(shockwave.riseSec, shockwave.hangSec, shockwave.fallSec);

        shockwave.Fire(
            origin: transform.position,
            tile: tileSize,
            radiusTiles: shockwave.radiusShock,
            targetLayer: shockwave.targetLayer,
            useOcclusion: shockwave.useOcclusion,
            occludeMask: shockwave.occluderMask,
            riseSeconds: shockwave.riseSec,
            hangSeconds: shockwave.hangSec,
            fallSeconds: shockwave.fallSec
        );

        // 감지탑 통지
        if (shockPing)
        {
            shockPing.PingTowers(transform.position);
            Debug.Log($"[Orb] 감지탑 Ping 전송 by {name}.", this);
        }
    }
}
