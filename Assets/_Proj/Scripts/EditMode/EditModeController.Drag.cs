using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 드래그 이동, 그리드 스냅, 겹침 체크, 바닥(Layer) 체크, Undo 스택
/// </summary>
public partial class EditModeController
{
    // 드래그 시작 직전 스냅
    private Snap? lastBeforeDrag;

    /// <summary>드래그용 평면 생성(Y 고정)</summary>
    private void PrepareMovePlane()
    {
        float y = fixedY;
        if (lockYToInitial && CurrentTarget)
            y = CurrentTarget.position.y;

        movePlaneY = y;
        movePlane = new Plane(Vector3.up, new Vector3(0f, movePlaneY, 0f));
        movePlaneReady = true;
    }

    /// <summary>스크린 좌표 기반으로 실제 월드 이동</summary>
    private void DragMove(Vector2 screenPos)
    {
        if (!cam) return;
        if (!ScreenPosValid(screenPos)) return;
        if (!movePlaneReady) PrepareMovePlane();

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (!movePlane.Raycast(ray, out float enter)) return;

        Vector3 hit = ray.GetPoint(enter);
        hit.y = movePlaneY;

        // 그리드 스냅
        if (snapToGrid)
            hit = SnapToGrid(hit);

        if (!CurrentTarget || CurrentTarget.position == hit) return;

        CurrentTarget.position = hit;
        movedDuringDrag = true;

        // 배치 유효성 검사
        bool onGround = IsOverGround(hit);
        bool noOverlap = !OverlapsOthers(CurrentTarget);
        bool valid = onGround && noOverlap;
        currentPlacementValid = valid;

        if (CurrentTarget.TryGetComponent<Draggable>(out var drag))
        {
            drag.SetInvalid(!valid);
            drag.SetHighlighted(true);
        }
    }

    /// <summary>드래그 종료 시 호출</summary>
    private void FinishDrag()
    {
        if (!IsEditMode || !CurrentTarget) return;

        // 1) 유효하지 않은 위치면 원래대로
        if (!currentPlacementValid)
        {
            if (lastBeforeDrag.HasValue)
            {
                CurrentTarget.position = lastBeforeDrag.Value.pos;
                CurrentTarget.rotation = lastBeforeDrag.Value.rot;
            }

            if (CurrentTarget.TryGetComponent<Draggable>(out var drag0))
            {
                drag0.SetInvalid(false);
                drag0.SetHighlighted(true);
            }
        }
        // 2) 유효 + 실제로 움직였으면 Undo 기록
        else if (movedDuringDrag && lastBeforeDrag.HasValue)
        {
            var stack = GetOrCreateHistory(CurrentTarget);
            stack.Push(lastBeforeDrag.Value);
            TrimHistoryIfNeeded(stack);

            hasUnsavedChanges = true;
            UpdateUndoUI();
        }

        movedDuringDrag = false;
        lastBeforeDrag = null;
        currentPlacementValid = true;
    }

    /// <summary>그리드 스냅</summary>
    private Vector3 SnapToGrid(Vector3 world)
    {
        float Snap(float v, float origin) => Mathf.Round((v - origin) / gridSize) * gridSize + origin;

        world.x = Snap(world.x, gridOrigin.x);
        world.z = Snap(world.z, gridOrigin.z);
        return world;
    }

    /// <summary>해당 위치가 Ground 레이어 위인지</summary>
    private bool IsOverGround(Vector3 worldPos)
    {
        if (!requireGround) return true;

        Vector3 origin = new Vector3(worldPos.x, worldPos.y + groundProbeUp, worldPos.z);
        float dist = groundProbeUp + groundProbeDown;

        return Physics.Raycast(origin, Vector3.down, out _, dist, groundMask, QueryTriggerInteraction.Ignore);
    }

    /// <summary>다른 오브젝트와 겹치는지 검사</summary>
    private bool OverlapsOthers(Transform t)
    {
        var myCols = t.GetComponentsInChildren<Collider>();
        if (myCols == null || myCols.Length == 0) return false;
        if (!TryGetCombinedBoundsFromColliders(myCols, out Bounds myBounds)) return false;

        var half = myBounds.extents;
        var center = myBounds.center;

        // 후보들만 모으기
        var candidates = Physics.OverlapBox(
            center,
            half,
            Quaternion.identity,
            draggableMask,
            QueryTriggerInteraction.Ignore
        );

        if (candidates == null || candidates.Length == 0) return false;

        foreach (var other in candidates)
        {
            if (!other || !other.enabled) continue;
            if (IsSameRootOrChild(t, other.transform)) continue;

            foreach (var my in myCols)
            {
                if (!my || !my.enabled) continue;
                if (my.isTrigger || other.isTrigger) continue;

                if (Physics.ComputePenetration(
                        my, my.transform.position, my.transform.rotation,
                        other, other.transform.position, other.transform.rotation,
                        out _, out float dist))
                {
                    if (dist > overlapEpsilon)
                        return true;
                }
            }
        }

        return false;
    }

    private static bool IsSameRootOrChild(Transform root, Transform other)
        => other == root || other.IsChildOf(root);

    private static bool TryGetCombinedBoundsFromColliders(Collider[] cols, out Bounds combined)
    {
        combined = new Bounds();
        bool hasAny = false;
        foreach (var c in cols)
        {
            if (!c || !c.enabled) continue;
            if (!hasAny) { combined = c.bounds; hasAny = true; }
            else combined.Encapsulate(c.bounds);
        }
        return hasAny;
    }

    #region === Undo ===

    public void UndoLastMove()
    {
        if (!CurrentTarget) return;

        if (history.TryGetValue(CurrentTarget, out var stack) && stack.Count > 0)
        {
            Snap prev = stack.Peek();

            // 현재값 백업
            Vector3 curPos = CurrentTarget.position;
            Quaternion curRot = CurrentTarget.rotation;

            // 되돌리기
            CurrentTarget.position = prev.pos;
            CurrentTarget.rotation = prev.rot;

            // 되돌렸더니 또 겹치면 취소
            if (OverlapsOthers(CurrentTarget))
            {
                CurrentTarget.position = curPos;
                CurrentTarget.rotation = curRot;

                if (CurrentTarget.TryGetComponent<Draggable>(out var dragFail))
                {
                    dragFail.SetInvalid(true);
                    dragFail.SetHighlighted(true);
                }
                Debug.Log("[Undo] 이전 상태가 겹쳐서 되돌릴 수 없습니다.");
                return;
            }

            stack.Pop();

            if (CurrentTarget.TryGetComponent<Draggable>(out var dragOk))
            {
                dragOk.SetInvalid(false);
                dragOk.SetHighlighted(true);
            }

            hasUnsavedChanges = true;
            Debug.Log("[Undo] 되돌리기 성공");
        }

        UpdateUndoUI();
    }

    public void ClearCurrentHistory()
    {
        if (!CurrentTarget) return;
        if (history.ContainsKey(CurrentTarget))
            history[CurrentTarget].Clear();
        UpdateUndoUI();
    }

    private Stack<Snap> GetOrCreateHistory(Transform t)
    {
        if (!history.TryGetValue(t, out var stack))
        {
            stack = new Stack<Snap>(8);
            history[t] = stack;
        }
        return stack;
    }

    private void TrimHistoryIfNeeded(Stack<Snap> stack)
    {
        if (undoMax <= 0) return;
        if (stack.Count <= undoMax) return;

        // Stack에는 최신이 위에 있어서 ToArray() 후 뒤집어서 잘라야 한다
        var arr = stack.ToArray();
        System.Array.Reverse(arr);
        int removeCount = stack.Count - undoMax;

        var trimmed = new List<Snap>(undoMax);
        for (int i = 0; i < arr.Length; i++)
        {
            if (i < removeCount) continue;
            trimmed.Add(arr[i]);
        }

        stack.Clear();
        for (int i = trimmed.Count - 1; i >= 0; i--)
            stack.Push(trimmed[i]);
    }

    private void UpdateUndoUI()
    {
        if (!undoButton) return;

        if (!IsEditMode)
        {
            undoButton.interactable = false;
            return;
        }

        bool canUndo = false;
        if (CurrentTarget && history.TryGetValue(CurrentTarget, out var stack))
            canUndo = stack != null && stack.Count > 0;

        undoButton.interactable = canUndo;
    }

    #endregion
}
