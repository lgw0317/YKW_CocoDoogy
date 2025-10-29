using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public static class DecoParser
{
    public static void Import(string csvPath)
    {
        string[] lines = File.ReadAllLines(csvPath);
        if (lines.Length <= 1) return;

        var db = ScriptableObject.CreateInstance<DecoDatabase>();
        if (db.decoList == null)
            db.decoList = new System.Collections.Generic.List<DecoData>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var v = line.Split(',');

            if (v.Length < 8)
            {
                Debug.LogWarning($"[DecoParser] {i}행 데이터 부족 → 스킵");
                continue;
            }

            // 첫 번째 값이 숫자가 아닐 경우 스킵
            if (!int.TryParse(v[0].Trim('\uFEFF'), out int id))
            {
                Debug.LogWarning($"[DecoParser] ID 변환 실패 → {v[0]}");
                continue;
            }

            int.TryParse(v[7], out int stack);

            // Enum 변환 부분
            Enum.TryParse(v[4], true, out Category category);
            Enum.TryParse(v[6], true, out Acquire acquire);

            db.decoList.Add(new DecoData
            {
                id = int.Parse(v[0]),
                name = v[1],
                prefabPath = v[2],
                iconPath = v[3],
                category = category,
                tag = v[5],
                acquire = acquire,
                stack = stack,
                description = v.Length > 8 ? v[8] : ""
            });
        }

        string assetPath = "Assets/_Proj/Data/ScriptableObject/Deco/DecoDatabase.asset";
        AssetDatabase.CreateAsset(db, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"[DecoParser] 변환 완료 → {assetPath}");
    }
}