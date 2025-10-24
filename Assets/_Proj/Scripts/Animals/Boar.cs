using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boar : PushableObjects, IDashDirection
{
    [Header("Canvas")]
    [SerializeField] GameObject btnGroup;
    [SerializeField] Button up;
    [SerializeField] Button down;
    [SerializeField] Button left;
    [SerializeField] Button right;
    [SerializeField] Image typeIcon;

    [Header("Player Detection")]
    public float detectRadius = 3f; // 플레이어 감지 범위
    public LayerMask playerLayer; // 플레이어 레이어 마스크
    private Transform playerTrans; // 감지된 플레이어의 Transform

    [Header("Boar Dash Settings")]
    public float dashSpeed = 0.08f; // 돌진 속도
    private DashExecutor dashExecutor;

    protected override void Awake()
    {
        base.Awake();

        // 버튼 그룹의 방향 버튼에 돌진 로직 연결
        up.onClick.AddListener(() => { GetDirection(new Vector2Int(0, 1)); btnGroup.SetActive(false); });
        down.onClick.AddListener(() => { GetDirection(new Vector2Int(0, -1)); btnGroup.SetActive(false); });
        left.onClick.AddListener(() => { GetDirection(new Vector2Int(-1, 0)); btnGroup.SetActive(false); });
        right.onClick.AddListener(() => { GetDirection(new Vector2Int(1, 0)); btnGroup.SetActive(false); });

        btnGroup.SetActive(false);
        
        dashExecutor = new DashExecutor(tileSize, blockingMask);

        // 플레이어 transform 찾기
        // NOTE : 플레이어 Tag를 Player로 설정
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerTrans = playerGO.transform;
        }
    }

    void Update()
    {
        if (isHoling && isMoving)
        {
            currHold += Time.deltaTime;
            if (currHold >= requiredHoldtime)
            {
                TryPush(holdDir);
                currHold = 0f;
                isHoling = false;
            }
        }       
        DetectPlayer();
    }

    // 플레이어 감지
    void DetectPlayer()
    {
        if (playerTrans == null) return;
        if (isMoving || isFalling)
        {
            if (btnGroup.activeSelf)
            {
                btnGroup.SetActive(false);
            }
            return;
        }
        Vector3 checkOrigin = transform.position + Vector3.up * 1f;

        // 플레이어까지의 거리 체크
        float distance = Vector3.Distance(checkOrigin, playerTrans.position);

        if (distance <= detectRadius) // 범위 안에 들어옴
        {
            if (!btnGroup.activeSelf)
            {
                btnGroup.SetActive(true);
            }
        }
        else // 범위에서 나감
        {
            if (btnGroup.activeSelf)
            {
                btnGroup.SetActive(false);
            }
        }
    }

    public void GetDirection(Vector2Int dashDir)
    {
        if (isMoving || isFalling) return;

        Vector3 moveDir = new Vector3(dashDir.x, 0, dashDir.y);
        StartCoroutine(DashCoroutine(moveDir, dashDir));
    }

    protected IEnumerator DashMoveTo(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < dashSpeed)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / dashSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    private IEnumerator DashCoroutine(Vector3 moveDir, Vector2Int chargeDir)
    {
        isMoving = true;

        while (true) // 끝없이 돌진
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = currentPos + moveDir * tileSize;

            if (CheckBlocking(nextPos))
            {
                if (Physics.Raycast(currentPos + Vector3.up * 0.5f, moveDir, out RaycastHit hit, tileSize + 0.05f, blockingMask))
                {
                    if (hit.distance <= tileSize && hit.collider.TryGetComponent<IPushHandler>(out var handler))
                    {
                        if (dashExecutor.CanChainPush(hit.collider.transform.position, chargeDir))
                        {
                            // 연쇄 밀어내기 실행 (밀린 박스/구체는 PushableObjects.moveTime으로 느리게 움직임)
                            dashExecutor.ExecuteChainPush(hit.collider.transform.position, chargeDir);

                            // 멧돼지 1칸 이동
                            yield return StartCoroutine(DashMoveTo(nextPos));

                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // 충돌 없이 한 칸 전진
            yield return StartCoroutine(DashMoveTo(nextPos));
        }

        isMoving = false;
        StartCoroutine(CheckFall()); // CheckFall은 부모의 MoveTo를 사용
    }

    protected override bool CheckBlocking(Vector3 target)
    {
        // 멧돼지 크기에 맞는 충돌체 검사
        return Physics.CheckBox(target + Vector3.up * 0.5f, Vector3.one * 0.4f, Quaternion.identity, blockingMask);
    }
}