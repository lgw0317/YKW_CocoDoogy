using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Buffalo : MonoBehaviour
{
    [Header("Timer & Jump")]
    [Tooltip("버튼 누른 뒤 실행까지 대기 시간")]
    public float interactionSeconds = 0.1f;
    [Tooltip("충격 전 점프 연출에 걸리는 시간")]
    public float jumpDuration = 0.35f;
    [Tooltip("점프 높이 곡선 0~1 비율")]
    public AnimationCurve jumpY = AnimationCurve.EaseInOut(0, 0, 1, 0.5f);
    // TODO : 버튼 쿨타임 필요(기본값 : 5초)

    [Header("Visual")]
    public RingRange ring;
    public Button interactionBtn;

    [Header("Player Detection")]
    public float detectRadius = 3f; // 플레이어 감지 범위
    public LayerMask playerLayer; // 플레이어 레이어 마스크
    private Transform playerTrans; // 감지된 플레이어의 Transform

    [SerializeField] private Shockwave shockwave;

    bool running;

    void Awake()
    {
        interactionBtn.gameObject.SetActive(false);
        ring.gameObject.SetActive(false);

        if (!shockwave) shockwave = GetComponent<Shockwave>();

        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerTrans = playerGO.transform;
        }
    }

    void Start()
    {
        if (ring == null) ring = GetComponentInChildren<RingRange>(true);
    }

    void Update()
    {
        DetectPlayer();
    }

    void LateUpdate()
    {
        if (interactionBtn)
        {
            // World Rotation을 Quaternion.identity(X=0, Y=0, Z=0)로 설정
            interactionBtn.transform.rotation = Quaternion.identity;
        }
    }

    // 플레이어 감지
    void DetectPlayer()
    {
        if (!playerTrans || !interactionBtn || !ring) return;

        float distance = Vector3.Distance(transform.position + Vector3.up * 0.5f, playerTrans.position);
        bool inRange = distance <= detectRadius;

        if (interactionBtn.gameObject.activeSelf != inRange)
        {
            interactionBtn.gameObject.SetActive(inRange);
            ring.gameObject.SetActive(inRange);
        }
    }

    public void Interact()
    {
        Debug.Log("[BuffaloShockwave] Interact called");
        if (!running) StartCoroutine(WaveRunCoroutine());
    }

    void OnMouseDown() => Interact();

    IEnumerator WaveRunCoroutine()
    {
        running = true;

        float tile = Mathf.Max(0.01f, shockwave? shockwave.tileHeight : 1f);

        // 타이머
        float t = 0f;
        while (t < interactionSeconds) { t += Time.deltaTime; yield return null; }

        // 점프
        yield return StartCoroutine(JumpCoroutine(tile));

        // 충격파
        shockwave.Fire();
        running = false;
    }

    IEnumerator JumpCoroutine(float tile)
    {
        var p0 = transform.position; // 시작 위치
        float t = 0f;
        while (t < jumpDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / jumpDuration); // 0~1 정규화 진행도
            float addY = jumpY.Evaluate(u) * tile; // 실제 상승량
            transform.position = new Vector3(p0.x, p0.y + addY, p0.z);
            yield return null;
        }
        transform.position = p0; // 끝나면 원위치
    }
}
