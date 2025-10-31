using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BackgroundParser
{
    public static void Import(string csvPath)
    {
        string textCsvPath = "Assets/_Proj/Data/CSV/tbl_text_mst.csv";
        var textDict = TextParser.Import(textCsvPath);

        string[] lines = File.ReadAllLines(csvPath);

        if (lines.Length <= 1) return;

        var db = ScriptableObject.CreateInstance<BackgroundDatabase>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line)) continue;

            var v = line.Split(',');

            if (v.Length < 6)
            {
                Debug.LogWarning($"[BackgroundParser] {i}행 데이터 부족 → 스킵");
                continue;
            }

            if (!int.TryParse(v[0].Trim('\uFEFF'), out int id))
            {
                Debug.LogWarning($"[BackgroundParser] ID 변환 실패 → {v[0]}");
                continue;
            }

            Enum.TryParse(v[2], true, out BackgroundTag tag);

            string rawName = v[1];
            string finalName = TextParser.Resolve(rawName, textDict);

            db.bgList.Add(new BackgroundData
            {
                bg_id = id,
                bg_name = finalName,
                bg_tag = tag,
                bg_icon = v[3],
                bg_skybox = v[4],
                bg_desc = v[5]
            });
        }

        string assetPath = "Assets/_Proj/Data/ScriptableObject/Background/BackgroundDatabase.asset";
        AssetDatabase.CreateAsset(db, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"[BackgroundParser] 변환 완료 → {assetPath}");
    }
}
