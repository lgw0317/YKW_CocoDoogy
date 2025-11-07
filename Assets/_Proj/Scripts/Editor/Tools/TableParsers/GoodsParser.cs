using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class GoodsParser
{
    public static void Import(string csvPath)
    {
        string textCsvPath = "Assets/_Proj/Data/CSV/tbl_goods_mst.csv";
        var textDict = TextParser.Import(textCsvPath);

        string[] lines = File.ReadAllLines(csvPath);

        if (lines.Length <= 1) return;

        var db = ScriptableObject.CreateInstance<GoodsDatabase>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line)) continue;

            var v = System.Text.RegularExpressions.Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (v.Length < 4)
            {
                Debug.LogWarning($"[GoodsParser] {i}행 데이터 부족 → 스킵");
                continue;
            }

            if (!int.TryParse(v[0].Trim('\uFEFF'), out int id))
            {
                Debug.LogWarning($"[GoodsParser] ID 변환 실패 → {v[0]}");
                continue;
            }


            string rawName = v[1];
            string finalName = TextParser.Resolve(rawName, textDict);

            string rawDesc = v[3];
            string finalDesc = TextParser.Resolve(rawDesc, textDict);

            db.goodsList.Add(new GoodsData
            {
                goods_id = id,
                goods_name = finalName,
                goods_icon = v[2],
                goods_desc = finalDesc
            });
        }

        string assetPath = "Assets/_Proj/Data/ScriptableObject/Goods/GoodsDatabase.asset";
        AssetDatabase.CreateAsset(db, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"[GoodsParser] 변환 완료 → {assetPath}");
    }
}
