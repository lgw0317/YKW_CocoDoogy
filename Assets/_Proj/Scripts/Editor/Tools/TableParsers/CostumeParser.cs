using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CostumeParser
{
    public static void Import(string csvPath)
    {
        string textCsvPath = "Assets/_Proj/Data/CSV/tbl_text_mst.csv";
        var textDict = TextParser.Import(textCsvPath);

        string[] lines = File.ReadAllLines(csvPath);

        if (lines.Length <= 1) return;

        var db = ScriptableObject.CreateInstance<CostumeDatabase>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line)) continue;

            var v = line.Split(',');

            if (v.Length < 8)
            {
                Debug.LogWarning($"[CostumeParser] {i}행 데이터 부족 → 스킵");
                continue;
            }

            if (!int.TryParse(v[0].Trim('\uFEFF'), out int id))
            {
                Debug.LogWarning($"[CostumeParser] ID 변환 실패 → {v[0]}");
                continue;
            }

            Enum.TryParse(v[4], true, out CostumePart part);
            Enum.TryParse(v[5], true, out CostumeTag tag);
            Enum.TryParse(v[6], true, out CostumeAcquire acquire);

            string rawName = v[1];
            string finalName = TextParser.Resolve(rawName, textDict);

            db.costumeList.Add(new CostumeData
            {
                costume_id = id,
                costume_name = finalName,
                costume_prefab = v[2],
                costume_icon = v[3],
                cos_part = part,
                cos_tag = tag,
                cos_acquire = acquire,
                costume_desc = v[7]
            });
        }

        string assetPath = "Assets/_Proj/Data/ScriptableObject/Costume/CostumeDatabase.asset";
        AssetDatabase.CreateAsset(db, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"[CostumeParser] 변환 완료 → {assetPath}");
    }
}
