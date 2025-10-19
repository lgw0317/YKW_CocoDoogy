using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField]
    private TileRegistry rg; // 레지스트리 Scriptable Objects
    [SerializeField]
    private string stageKey = "Stages/Stage-01"; // Resources 경로 키

    // 같은 channel 끼리 모아두는 맵
    private readonly Dictionary<string, List<GameObject>> channelMap = new();

    public async Task LoadFromReourcesAsync(string key = null)
    {
        if (!string.IsNullOrEmpty(key)) stageKey = key;

        // Load JSON
        var json = await StageLocator.LoadAsync(stageKey);
        if (string.IsNullOrEmpty(json)) return;

        // Parsing DTO
        var level = JsonUtility.FromJson<LevelDTO>(json);
        if (level?.grid == null) return;

        ClearScene();

        // Validate Tiles
        if (!ValidateTiles(level)) return;

        // Create Tiles
        for (int r = 0; r < level.grid.rows; r++)
        {
            var tokens = level.tiles[r].Split(',');
            for (int c = 0; c < level.grid.cols; c++)
            {
                var code = tokens[c].Trim();
                var h = rg.LoadTilePrefabAsync(code);
                h.Completed += op =>
                {
                    if (!op.IsValid() || op.Result == null) return;

                    var pos = ToWorld(c, r, level.grid);
                    Instantiate(op.Result, pos, Quaternion.identity, transform);
                };
            }
        }

        // Set Entities
        if (level.entities != null)
        {
            foreach (var e in level.entities)
            {
                var h = rg.LoadEntityPrefabAsync(e.type);
                h.Completed += op =>
                {
                    if (!op.IsValid() || op.Result == null) return;

                    var go = Instantiate(op.Result, ToWorld(e.x, e.y, level.grid), Quaternion.identity, transform);
                    ApplyFwd(go.transform, e.initFwd);
                };
            }
        }
    }

    private void ClearScene()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
        channelMap.Clear();
        rg.Release(default);
    }

    private async void Start()
    {
        if (rg == null) return;
        await LoadFromReourcesAsync(stageKey);
    }

    private bool ValidateTiles(LevelDTO level)
    {
        if (level.tiles == null || level.tiles.Count != level.grid.rows) return false;
        for (int r = 0; r < level.grid.rows; r++)
        {
            var cnt = level.tiles[r].Split(',').Length;
            if (cnt != level.grid.cols) return false;
        }
        return true;
    }

    private static Vector3 ToWorld(int x, int y, GridDTO g)
        => g.origin + new Vector3(x * g.tileSize, 0, y * g.tileSize);

    private static void ApplyFwd(Transform t, InitFwd fwd)
    {

        float yaw = fwd switch
        {
            InitFwd.Up => 0,
            InitFwd.Right => 90f,
            InitFwd.Down => 180f,
            InitFwd.Left => 270f,
            _ => 0,
        };
        t.rotation = Quaternion.Euler(0, yaw, 0);
    }

}
