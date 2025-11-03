using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO : 빙판 로직 같은 수상 택시
// 지정된 방향으로 n초간 이동
// 위에 고정X&통과X 물체 태울 수 있음.
// 통과X에 부딪히면 멈춤.(물/물길만 다닐 수 있음)
// 움직이는 동안은 팝업X
// 물길 영향 X
[RequireComponent(typeof(Rigidbody))]
public class Turtle : MonoBehaviour, IDashDirection, IPlayerFinder
{
    [Header("Movement Settgins")]
    public float tileSize = 1f;
    [Tooltip("이동 속도 (tile/s)")]
    public float moveSpeed = 5f;
    [Tooltip("거북이 충돌 레이어")]
    public LayerMask blockLayer;
    [Tooltip("터틀 위에 올라탈 수 있는 오브젝트의 레이어 이름")]
    public LayerMask ridableLayer;

    [Header("Player Detection")]
    public float detectRadius = 3f; // 플레이어 감지 범위
    public LayerMask playerLayer; // 플레이어 레이어 마스크
    private Transform playerTrans; // 감지된 플레이어의 Transform

    private bool isMoving = false;
    private Vector3 targetPos;
    private Rigidbody rb; // 물리 처리용 (물살 영향 방지)

    [Header("Popup Buttons")]
    [SerializeField] private GameObject btnGroup;
    [SerializeField] private Button up;
    [SerializeField] private Button down;
    [SerializeField] private Button left;
    [SerializeField] private Button right;
    [SerializeField] Image typeIcon;

    public bool CanInteract => !isMoving; // 이동 중이 아닐 때만 상호작용 가능

    Transform IPlayerFinder.Player { get => playerTrans; set => playerTrans = value; }

    void Awake()
    {
        up.onClick.AddListener(() => { GetDirection(new Vector2Int(0, 1)); btnGroup.SetActive(false); });
        down.onClick.AddListener(() => { GetDirection(new Vector2Int(0, -1)); btnGroup.SetActive(false); });
        left.onClick.AddListener(() => { GetDirection(new Vector2Int(-1, 0)); btnGroup.SetActive(false); });
        right.onClick.AddListener(() => { GetDirection(new Vector2Int(1, 0)); btnGroup.SetActive(false); });

        btnGroup.SetActive(false);

        // 플레이어 transform 찾기
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerTrans = playerGO.transform;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
    
    void Update()
    {
        if (!isMoving)
        {
            DetectPlayer();
        }
    }

    void DetectPlayer()
    {
        if (!playerTrans || !btnGroup) return;

        float dist = Vector3.Distance(transform.position + Vector3.up * 0.5f, playerTrans.position);
        bool inRange = dist <= detectRadius;

        if (btnGroup.activeSelf != inRange)
        {
            btnGroup.SetActive(inRange);
            if (inRange)
            {
                // NOTE : UI 월드 축 고정되게 설정함. 0,0,0 추후 변경.
                btnGroup.transform.rotation = Quaternion.identity;
            }
        }
    }

    public void GetDirection(Vector2Int dir)
    {
        if (isMoving) return;

        isMoving = true;
        btnGroup.SetActive(false);

        Vector3 moveDir = new Vector3(dir.x, 0, dir.y);

        // 빙판 로직 적용 : 멈출 때까지 방향으로 이동할 목표 위치 계산
        targetPos = CalculateSlideTarget(moveDir);

        // 이동 시작 시, 위에 얹힌 모든 오브젝트도 같이 이동 시작
        StartCoroutine(MoveSlideCoroutine(moveDir, transform.position, targetPos));
    }

    // 빙판 로직 계산
    private Vector3 CalculateSlideTarget(Vector3 dir)
    {
        Vector3 currTile = transform.position;
        Vector3 nextTile = currTile + dir * tileSize;

        // 충돌 체크용 박스 크기
        Vector3 boxHalfExt = Vector3.one * (tileSize * 0.45f) + Vector3.up * 0.25f;

        // Block Layer를 만날 때까지 계속 이동 (빙판 로직)
        while (true)
        {
            bool isBlocking = Physics.CheckBox(nextTile + Vector3.up * (tileSize / 2),
                boxHalfExt,
                Quaternion.identity,
                blockLayer,
                QueryTriggerInteraction.Ignore
            );
            if (isBlocking)
            {
                break;
            }
            currTile = nextTile;
            nextTile += dir * tileSize;
        }
        return currTile; // 블록 타일 바로 앞 타일 위치 반환
    }

    // 탑승 물체 이동
    // HACK : 이 부분은 움직일 수 있는 물체가 Turtle에 안 막히게 하면 될 것 같기도 한데 어떤 부분이 효율적일지 고민해봐야 함. 둘 다 처리 하는 게 안전한 방법일 수도.
    IEnumerator MoveSlideCoroutine(Vector3 dir, Vector3 startPos, Vector3 endPos)
    {
        float dist = Vector3.Distance(startPos, endPos);

        if (dist < tileSize * 0.5f)
        {
            isMoving = false;
            yield break;
        }

        float duration = dist / moveSpeed; // 이동에 필요한 시간
        float elapsed = 0f;

        // 탑승 가능한 오브젝트 감지
        Vector3 overlapOrigin = transform.position + Vector3.up * (tileSize / 4f);
        Collider[] ridables = Physics.OverlapBox(
            overlapOrigin,
            Vector3.one * (tileSize / 2f),
            Quaternion.identity,
            ridableLayer,
            QueryTriggerInteraction.Ignore
        );

        List<Transform> ridableTrans = new List<Transform>();
        List<Vector3> ridableStartPos = new List<Vector3>();
        List<Vector3> ridableTargetPos = new List<Vector3>();

        Vector3 offset = endPos - startPos; // 터틀의 전체 이동 변위

        foreach (var col in ridables)
        {
            // Transform을 직접 이동 대상에 추가
            if (col.transform != transform)
            {
                ridableTrans.Add(col.transform);
                ridableStartPos.Add(col.transform.position);
                ridableTargetPos.Add(col.transform.position + offset); // 터틀의 이동 변위만큼 목표 위치 설정
            }
        }

        // 터틀과 탑승 물체 동시 이동
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 터틀 이동
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // 탑승 물체 이동
            for (int i = 0; i < ridableTrans.Count; i++)
            {
                // Transform을 직접 조작하여 동기화
                ridableTrans[i].position = Vector3.Lerp(ridableStartPos[i], ridableTargetPos[i], t);
            }

            yield return null;
        }

        // 이동 완료 및 상태 정리
        transform.position = endPos;
        for (int i = 0; i < ridableTrans.Count; i++)
        {
            ridableTrans[i].position = ridableTargetPos[i];
        }

        isMoving = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // 플레이어 감지 범위
        Gizmos.color = Color.yellow * 0.5f;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, detectRadius);

        // Ridable 오브젝트 감지 범위
        Gizmos.color = Color.cyan * 0.5f;
        Vector3 overlapOrigin = transform.position + Vector3.up * (tileSize / 4f);
        Gizmos.DrawWireCube(overlapOrigin, Vector3.one * tileSize / 2f);

        // 최종 목표 위치
        if (Application.isPlaying && isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPos, 0.2f);
        }
    }
#endif
}