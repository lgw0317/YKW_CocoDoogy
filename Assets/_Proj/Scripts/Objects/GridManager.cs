using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    private Dictionary<Vector2Int, GameObject> grid = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x, gridPos.y);
    }

    public void Register(Vector2Int gridPos, GameObject obj)
    {
        grid[gridPos] = obj;
    }
    public void Unregister(Vector2Int gridPos) {
        if (grid.ContainsKey(gridPos))
        {
            grid.Remove(gridPos);
        }
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return grid.ContainsKey(pos);
    }

    public GameObject GeteObjectAt(Vector2Int pos)
    {
        return grid.TryGetValue(pos, out var obj) ? obj : null;
    }
}
