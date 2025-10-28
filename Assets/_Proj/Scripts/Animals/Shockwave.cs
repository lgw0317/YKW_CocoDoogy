using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 충격파 발생
[DisallowMultipleComponent]
public class Shockwave : MonoBehaviour
{
    [Header("Shockwave Settings")]
    [Min(1f)] public float tileHeight = 1f; // 타일 한 칸 높이
    [Tooltip("충격파 반경 ==> 중요!! : 여기 바뀌면 RingRange 컴포넌트의 범위도 같이 바꿔줘야 함!!!")]
    [Range(1f, 7f)] public float radiusShock = 5f;
    [Tooltip("충격파 영향 받는 Pushables가 포함된 레이어")]
    public LayerMask targetLayer = ~0;

    // HACK : 이건 따로 빼서 차폐물 자체에 붙여 주는 형식으로 변경하는 것이 나을 지도
    [Header("Occlusion")]
    [Tooltip("충격파 차폐 될 지 여부")]
    public bool useOcclusion = false;
    [Tooltip("차폐 인정할 레이어")]
    public LayerMask occluderMask = ~0;

    [Header("Lift Timing")]
    [Tooltip("떠오르는 데 걸리는 시간")]
    [Min(0.01f)] public float riseSecondsPerTile = 0.5f;
    [Tooltip("공중에서 홀딩되는 시간")]
    [Min(0.0f)] public float hangSeconds = 0.4f;

    public void Fire()
    {
        Fire(
            origin: transform.position,
            tile: tileHeight,
            radiusTiles: radiusShock,
            targetLayer: targetLayer,
            useOcclusion: useOcclusion,
            occludeMask: occluderMask,
            riseSecondsPerTile: riseSecondsPerTile,
            hangSeconds: hangSeconds
         );
    }

    public void Fire(
        Vector3 origin,
        float tile,
        float radiusTiles,
        LayerMask targetLayer,
        bool useOcclusion,
        LayerMask occludeMask,
        float riseSecondsPerTile,
        float hangSeconds
    )
    {
        float t = Mathf.Max(0.0001f, tile);
        float radiusWorld = Mathf.Max(0.0001f, radiusTiles) * t;

        // 중심 레벨(Y) 스냅
        int centerH = Mathf.FloorToInt(origin.y / t + 1e-4f);

        // 반경 내 후보 수집
        var cols = Physics.OverlapSphere(origin, radiusWorld, targetLayer, QueryTriggerInteraction.Ignore);
        if (cols == null || cols.Length == 0) return;

        var all = new List<(Collider col, Component pushable, int h, int gx, int gz, float ts)>();
        foreach (var c in cols)
        {
            var p = (Component)c.GetComponent("PushableObjects")
                  ?? (Component)c.GetComponent("PushableBox")
                  ?? (Component)c.GetComponent("PushableOrb");

            float ts = p != null ? GetTileSize(p, tile) : tile; // 비-푸시블은 기본 타일 높이
            int h = Mathf.FloorToInt(c.transform.position.y / ts + 1e-4f);
            int gx = Mathf.FloorToInt((c.transform.position.x + 0.5f * ts) / ts);
            int gz = Mathf.FloorToInt((c.transform.position.z + 0.5f * ts) / ts);
            all.Add((c, p, h, gx, gz, ts));
        }
        // 기준층(centerH) 가시성 검사 (pushables도 occlude)
        LayerMask blockMask = occluderMask | targetLayer;
        var baseHits = new List<(Collider col, Component pushable, int gx, int gz, float ts)>();
        foreach (var e in all)
        {
            if (e.h != centerH) continue;

            if (useOcclusion && IsLineBlocked(origin, e.col, centerH, e.ts, blockMask, this.transform))
                continue;
            baseHits.Add((e.col, e.pushable, e.gx, e.gz, e.ts));
        }

        // 컬럼 전파: 같은 (gx,gz)에서 h >= centerH 전부 리프트
        foreach (var baseHit in baseHits)
        {
            int gx = baseHit.gx;
            int gz = baseHit.gz;

            foreach (var e in all)
            {
                if (e.gx != gx || e.gz != gz) continue;
                if (e.h < centerH) continue; // 아래층 제외

                float rise = Mathf.Max(0.0001f, riseSecondsPerTile);
                float hang = Mathf.Max(0f, hangSeconds);

                if (e.pushable != null)
                {
                    if (!TryCallLiftAny(e.pushable, rise, hang))
                        StartCoroutine(LiftTransCoroutine(e.col.transform, e.ts, rise, hang));
                }
                else
                {
                    // layer로 타깃되면 폴백 리프트 허용
                    StartCoroutine(LiftTransCoroutine(e.col.transform, e.ts, rise, hang));
                }
            }
        }
    }

    static bool TryCallLiftAny(Component target, float rise, float hang)
    {
        const string methodName = "WaveLift";
        var type = target.GetType();
        var methods = type.GetMethods().Where(m => m.Name == methodName);

        foreach (var m in methods)
        {
            var ps = m.GetParameters();
            object[] args = null;

            if (ps.Length == 2) args = new object[] { ConvertArg(ps[0], rise), ConvertArg(ps[1], hang) };
            else if (ps.Length == 1) args = new object[] { ConvertArg(ps[0], rise) };
            else if (ps.Length == 0) args = null;
            else continue;

            try { m.Invoke(target, args); return true; }
            catch { /* 다음 시그니처 시도 */ }
        }
        return false;
    }

    static bool IsLineBlocked(
    Vector3 origin, // 충격파 발사 원점
    Collider targetCol, // 리프트 대상
    int centerH,
    float tile,
    LayerMask blockMask, // occludeMask | targetLayer
    Transform ignoreRoot // 발사자(this.transform)
    )
    {
        // 중심 높이에서 가시선 검사
        float yCentre = centerH * tile + tile * 0.5f;
        float halfBand = tile * 0.45f; // 밴드 두께

        Vector3 a = new(origin.x, yCentre, origin.z);
        Vector3 b = new(targetCol.transform.position.x, yCentre, targetCol.transform.position.z);
        Vector3 dir = b - a;
        float dist = dir.magnitude;
        if (dist <= 0.0001f) return false;
        dir /= dist;

        var hits = Physics.RaycastAll(a, dir, dist - 0.01f, blockMask, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            var tr = hit.collider.transform;

            // 발사자/자식 무시
            if (ignoreRoot && (tr == ignoreRoot || tr.IsChildOf(ignoreRoot))) continue;
            // 타깃 자신/자식 무시
            if (hit.collider == targetCol) continue;
            if (tr == targetCol.transform || tr.IsChildOf(targetCol.transform)) continue;

            // 충격파 밴드와 수직으로 겹치지 않으면 무시
            var cb = hit.collider.bounds;
            if (cb.max.y < yCentre - halfBand) continue;
            if (cb.min.y > yCentre + halfBand) continue;

            return true; // 밴드 안에서 라인 가림
        }
        return false;
    }

    static object ConvertArg(System.Reflection.ParameterInfo pi, float value)
    {
        if (pi.ParameterType == typeof(int)) return Mathf.RoundToInt(value);
        if (pi.ParameterType == typeof(float)) return value;
        return null;
    }

    static float GetTileSize(Component p, float fallback)
    {
        var t = p.GetType();
        var f = t.GetField("tileSize");
        if (f != null && f.FieldType == typeof(float))
            return Mathf.Max(0.0001f, (float)f.GetValue(p));

        var prop = t.GetProperty("tileSize");
        if (prop != null && prop.PropertyType == typeof(float))
            return Mathf.Max(0.0001f, (float)prop.GetValue(p));

        return Mathf.Max(0.0001f, fallback);
    }

    static System.Collections.IEnumerator LiftTransCoroutine(Transform tr, float tile, float rise, float hang)
    {
        Vector3 start = tr.position;
        Vector3 upPos = start + Vector3.up * tile;

        // 상승
        float t = 0f;
        while (t < rise)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / rise);
            tr.position = Vector3.Lerp(start, upPos, k);
            yield return null;
        }
        tr.position = upPos;

        if (hang > 0f) yield return new WaitForSeconds(hang);

        // 낙하는 대상 PushableObjects 측에서 처리 X
        // 원위치로 하강
        t = 0f;
        while(t < rise)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / rise);
            tr.position = Vector3.Lerp(upPos, start, k);
            yield return null;
        }
        tr.position = start;
    }

    static bool HasFixedAboveAny(List<Component> stack, int upTiles)
    {
        foreach (var p in stack)
        {
            float ts = GetTileSize(p, 1f);
            Vector3 center = p.transform.position + Vector3.up * (upTiles * ts);
            float r = ts * 0.25f;

            var cols = Physics.OverlapSphere(center, r, ~0, QueryTriggerInteraction.Ignore);
            foreach (var c in cols)
            {
                var so = c.GetComponent<ShockwaveObject>();
                if (so != null && so.isFixed && !so.passThrough) return true;
            }
        }
        return false;
    }

    static bool IsBlocked(Vector3 from, Vector3 to, int centerH, float tile, LayerMask occludeMask)
    {
        float y = centerH * tile + tile * 0.5f;
        Vector3 a = new(from.x, y, from.z);
        Vector3 b = new(to.x, y, to.z);
        Vector3 dir = (b - a);
        float dist = dir.magnitude;
        if (dist <= 0.0001f) return false;
        dir /= dist;

        var hits = Physics.RaycastAll(a, dir, dist, occludeMask, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            var sw = h.collider.GetComponent<ShockwaveObject>();
            if (sw != null)
            {
                // 컴포넌트가 있으면 속성 우선
                if (sw.passThrough) continue; // 통과 허용
                if (sw.isFixed) return true; // 고정물 -> 차폐
                continue;
            }

            // 컴포넌트가 없어도 occludeMask에 맞으면 차폐된 것으로 간주
            return true;
        }
        return false;
    }
}
