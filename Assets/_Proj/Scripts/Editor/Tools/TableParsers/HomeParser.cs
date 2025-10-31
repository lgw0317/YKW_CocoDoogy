using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class HomeParser
{
    public static void Import(string csvPath)
    {
        string textCsvPath = "Assets/_Proj/Data/CSV/tbl_text_mst.csv";
        var textDict = TextParser.Import(textCsvPath);

        string[] lines = File.ReadAllLines(csvPath);

        if (lines.Length <= 1) return;

        var db = ScriptableObject.CreateInstance<HomeDatabase>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line)) continue;

            var v = line.Split(',');

            if (v.Length < 7)
            {
                Debug.LogWarning($"[HomeParser] {i}행 데이터 부족 → 스킵");
                continue;
            }

            if (!int.TryParse(v[0].Trim('\uFEFF'), out int id))
            {
                Debug.LogWarning($"[HomeParser] ID 변환 실패 → {v[0]}");
                continue;
            }

            Enum.TryParse(v[5], true, out HomeTag tag);
            Enum.TryParse(v[6], true, out HomeAcquire acquire);

            string rawName = v[1];
            string finalName = TextParser.Resolve(rawName, textDict);

            db.homeList.Add(new HomeData
            {
                home_id = id,
                home_name = finalName,
                home_prefab = v[2],
                home_icon = v[3],
                home_tag = tag,
                home_acquire = acquire,
                home_desc = v[6]
            });
        }

        string assetPath = "Assets/_Proj/Data/ScriptableObject/Home/HomeDatabase.asset";
        AssetDatabase.CreateAsset(db, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"[HomeParser] 변환 완료 → {assetPath}");
    }
}
