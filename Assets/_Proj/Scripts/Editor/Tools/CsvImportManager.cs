using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CsvImportManager : EditorWindow
{
    [MenuItem("Tools/CSV Import Manager")]
    public static void ShowWindow() => GetWindow(typeof(CsvImportManager));

    private string csvFolderPath = "Assets/_Proj/Data/CSV";
    private string metaFilePath = "Assets/_Proj/Scripts/Editor/Tools/table_meta.json";

    void OnGUI()
    {
        if (GUILayout.Button("CSV → ScriptableObject 변환"))
        {
            ImportAllTables();
        }
    }

    void ImportAllTables()
    {
        // JsonUtility로 메타 읽기
        string metaJson = File.ReadAllText(metaFilePath);
        var metaList = JsonUtility.FromJson<TableMetaList>(metaJson);

        foreach (string csvPath in Directory.GetFiles(csvFolderPath, "*.csv"))
        {
            string fileName = Path.GetFileNameWithoutExtension(csvPath);

            // meta에 존재하는지 확인
            var meta = metaList.entries.Find(x => x.name == fileName);
            if (meta == null)
            {
                Debug.Log($"[CSV] {fileName} 은 meta.json에 정의되어 있지 않음 → 건너뜀");
                continue;
            }

            if (meta.type != "SO") continue;

            // 테이블 이름별 전용 파서 호출
            switch (fileName)
            {
                case "tbl_deco_mst":
                    DecoParser.Import(csvPath);
                    break;
                default:
                    Debug.LogWarning($"[CSV] {fileName} 변환 로직 없음");
                    break;
            }
        }
    }
}