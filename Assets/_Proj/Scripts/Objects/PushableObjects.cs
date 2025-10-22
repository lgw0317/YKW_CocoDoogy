using System.Collections;
using UnityEngine;

public class PushableObjects : MonoBehaviour
{
    public float moveTime = 0.12f;
    public float tileSize = 1f;
    public LayerMask blockingMask;

    private bool isMoving = false;

    public float requiredHoldtime = 0.5f;
    private float currHold = 0f;
    private Vector2Int holdDir;
    private bool isHoling = false;

    void Update()
    {
        if (!isHoling || isMoving) return;
        currHold += Time.deltaTime;
        if(currHold >= requiredHoldtime)
        {
            TryPush(holdDir);
            currHold = 0f;
            isHoling = false;
        }
    }

    public bool TryPush(Vector2Int dir)
    {
        if (isMoving) return false;

        Vector3 offset = new Vector3(dir.x, 0f, dir.y) * tileSize;
        Vector3 target = transform.position + offset;

        // 목적지에 뭔가 있으면 못 감(레이어 설정)
        if (Physics.CheckBox(target + Vector3.up * 0.5f, Vector3.one * 0.4f, Quaternion.identity, blockingMask))
            return false;

        StartCoroutine(MoveTo(target));
        return true;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    public void StartPushAttempt(Vector2Int dir)
    {
        if(isMoving) return;
        if(isHoling && dir != holdDir)
        {
            currHold = 0f;
        }

        holdDir = dir;
        isHoling = true;
    }

    public void StopPushAttempt()
    {
        isHoling = false;
        currHold = 0f;
    }
}
