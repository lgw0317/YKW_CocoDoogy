using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableStore : MonoBehaviour
{
    public static PlaceableStore I { get; private set; }
    [SerializeField] private HomeDatabase homeDB;
    [SerializeField] private AnimalDatabase animalDB;
    [SerializeField] private DecoDatabase decoDB;

    private const string PREF_KEY = "PlaceableStore_v1";

    [Serializable]
    public struct Placed
    {
        public PlaceableCategory cat;
        public int id;
        public Vector3 pos;
        public Quaternion rot;
    }
    [Serializable]
    private class Wrapper { public List<Placed> items; }

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        LoadAndSpawnAll();
    }

    public void SaveAllFromScene()
    {
#if UNITY_2022_2_OR_NEWER
        var tags = FindObjectsByType<PlaceableTag>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
        var tags = FindObjectsOfType<PlaceableTag>();
#endif
        var list = new List<Placed>();
        foreach (var tag in tags)
        {
            if (!tag || !tag.gameObject.activeInHierarchy) continue;
            if (tag.GetComponent<InventoryTempMarker>()) continue; // 임시는 제외

            list.Add(new Placed
            {
                cat = tag.category,
                id = tag.id,
                pos = tag.transform.position,
                rot = tag.transform.rotation
            });
        }

        string json = JsonUtility.ToJson(new Wrapper { items = list }, false);
        PlayerPrefs.SetString(PREF_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[PlaceableStore] Saved {list.Count} placed objects.");
    }

    private void LoadAndSpawnAll()
    {
        if (!PlayerPrefs.HasKey(PREF_KEY)) return;
        string json = PlayerPrefs.GetString(PREF_KEY, "");
        if (string.IsNullOrEmpty(json)) return;

        var w = JsonUtility.FromJson<Wrapper>(json);
        if (w?.items == null) return;

        var loader = new ResourcesLoader();

        foreach (var p in w.items)
        {
            GameObject prefab = null;
            string name = $"Item {p.id}";

            switch (p.cat)
            {
                case PlaceableCategory.Home:
                    var hd = homeDB?.homeList.Find(d => d != null && d.home_id == p.id);
                    prefab = hd?.GetPrefab(loader);
                    name = hd?.home_name ?? name;
                    break;
                case PlaceableCategory.Animal:
                    var ad = animalDB?.animalList.Find(d => d != null && d.animal_id == p.id);
                    prefab = ad?.GetPrefab(loader);
                    name = ad?.animal_name ?? name;
                    break;
                case PlaceableCategory.Deco:
                    var dd = decoDB?.decoList.Find(d => d != null && d.deco_id == p.id);
                    prefab = dd?.GetPrefab(loader);
                    name = dd?.deco_name ?? name;
                    break;
            }
            if (!prefab) { Debug.LogWarning($"[PlaceableStore] Prefab not found: {p.cat}:{p.id}"); continue; }

            var go = Instantiate(prefab, p.pos, p.rot);
            go.name = name;

            var tag = go.GetComponent<PlaceableTag>() ?? go.AddComponent<PlaceableTag>();
            tag.category = p.cat;
            tag.id = p.id;

            if (!go.GetComponent<Draggable>()) go.AddComponent<Draggable>();
        }

        Debug.Log($"[PlaceableStore] Restored {w.items.Count} placed objects.");
    }
}
