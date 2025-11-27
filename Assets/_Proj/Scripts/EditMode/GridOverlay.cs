using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class GridOverlay : MonoBehaviour
{
    [Header("Target Ground")]
    [SerializeField] private Renderer groundRenderer;   // Grid를 깔 기준이 되는 Plane / Mesh
    [SerializeField] private float cellSize = 1f;       // 칸 크기
    [SerializeField] private int maxCellsPerAxis = 200; // 너무 넓을 때 폭주 방지

    [Header("Visual")]
    [SerializeField] private Material lineMaterial;     // URP Unlit/Color 머티리얼
    [SerializeField] private Color lineColor = new Color(1, 1, 1, 0.25f);
    [SerializeField] private float lineWidth = 0.02f;
    [SerializeField] private float yOffset = 0.02f;     // Ground 위로 살짝 띄우기

    private readonly List<LineRenderer> _lines = new();
    private bool _built = false;
    private bool _visible = false;

    private void Awake()
    {
        // 처음 씬 들어올 때는 항상 비표시 상태
        SetVisible(false);
    }

    private void OnDisable()
    {
        SetVisible(false);
    }

    // ─────────────────────────────────────────────
    // Public API (EditModeController에서 호출)
    // ─────────────────────────────────────────────

    /// <summary>편집모드 On → Grid 표시</summary>
    public void Show()
    {
        EnsureGround();
        EnsureBuilt();
        SetVisible(true);
    }

    /// <summary>편집모드 Off → Grid 숨김</summary>
    public void Hide()
    {
        SetVisible(false);
    }

    // ─────────────────────────────────────────────
    // Build / Visible
    // ─────────────────────────────────────────────

    private void EnsureGround()
    {
        if (groundRenderer != null) return;

        // 혹시 비워놨으면, 자기 자식/부모에서 아무 Renderer나 하나라도 잡아오기
        groundRenderer = GetComponentInChildren<Renderer>();
        if (!groundRenderer)
        {
            Debug.LogWarning("[GridOverlay] groundRenderer가 설정되지 않았습니다.");
        }
    }

    /// <summary>아직 라인을 만든 적이 없다면 지금 만든다.</summary>
    private void EnsureBuilt()
    {
        if (_built) return;
        if (!groundRenderer) return;

        BuildGridFromBounds();
        _built = true;
    }

    private void BuildGridFromBounds()
    {
        Bounds b = groundRenderer.bounds;

        float minX = b.min.x;
        float maxX = b.max.x;
        float minZ = b.min.z;
        float maxZ = b.max.z;

        if (cellSize <= 0.001f)
            cellSize = 1f;

        int xCount = Mathf.CeilToInt((maxX - minX) / cellSize) + 1;
        int zCount = Mathf.CeilToInt((maxZ - minZ) / cellSize) + 1;

        xCount = Mathf.Min(xCount, maxCellsPerAxis);
        zCount = Mathf.Min(zCount, maxCellsPerAxis);

        int neededLines = xCount + zCount;

        // 라인 개수 맞추기
        while (_lines.Count < neededLines)
        {
            var go = new GameObject("GridLine");
            go.transform.SetParent(transform, worldPositionStays: false);
            var lr = go.AddComponent<LineRenderer>();
            SetupLineRenderer(lr);
            _lines.Add(lr);
        }

        // 초과분은 비활성
        for (int i = neededLines; i < _lines.Count; i++)
        {
            if (_lines[i])
                _lines[i].gameObject.SetActive(false);
        }

        int index = 0;
        float y = b.min.y + yOffset;

        // Z 방향으로 평행한 X-라인들
        for (int xi = 0; xi < xCount; xi++)
        {
            float x = minX + xi * cellSize;
            var lr = _lines[index++];
            if (!lr) continue;

            lr.gameObject.SetActive(true);
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(x, y, minZ));
            lr.SetPosition(1, new Vector3(x, y, maxZ));
        }

        // X 방향으로 평행한 Z-라인들
        for (int zi = 0; zi < zCount; zi++)
        {
            float z = minZ + zi * cellSize;
            var lr = _lines[index++];
            if (!lr) continue;

            lr.gameObject.SetActive(true);
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(minX, y, z));
            lr.SetPosition(1, new Vector3(maxX, y, z));
        }
    }

    private void SetupLineRenderer(LineRenderer lr)
    {
        lr.useWorldSpace = true;
        lr.alignment = LineAlignment.View;
        lr.shadowCastingMode = ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

        lr.widthMultiplier = lineWidth;
        lr.numCornerVertices = 0;
        lr.numCapVertices = 0;

        if (lineMaterial)
            lr.sharedMaterial = lineMaterial;

        lr.startColor = lineColor;
        lr.endColor = lineColor;
    }

    private void SetVisible(bool on)
    {
        _visible = on;
        for (int i = 0; i < _lines.Count; i++)
        {
            var lr = _lines[i];
            if (!lr) continue;
            lr.gameObject.SetActive(on && _built);
        }
    }
}
