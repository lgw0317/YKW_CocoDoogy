using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System;

public class CsvImportManager : EditorWindow
{
    [MenuItem("Tools/CSV Import Manager")]
    public static void ShowWindow() => GetWindow(typeof(CsvImportManager));
    //유니티 에디터 상에서 Tools -> CSV Import Manager 생성

    private string metaFilePath = "Assets/_Proj/Scripts/Editor/Tools/table_meta.json";
    //각각의 데이터 관련 정보를 보관하는 json이 보관된 경로
    
    void OnGUI()
    {
        GUILayout.Label("CSV 자동 다운로드 + Import", EditorStyles.boldLabel);
        metaFilePath = EditorGUILayout.TextField("Meta JSON Path", metaFilePath);

        if (GUILayout.Button("Download & Import All"))
        {
            DownloadAndImport();
            //에디터에 생성한 CSV Import Manager의 버튼 생성 및 수행할 함수 할당
        }
    }
    private void DownloadAndImport()
    {
        if (!File.Exists(metaFilePath))
        {
            Debug.LogError("Meta 파일을 찾을 수 없습니다: " + metaFilePath);
            return;
        }

        string json = File.ReadAllText(metaFilePath);
        TableMetaList config = JsonUtility.FromJson<TableMetaList>(json);

        foreach (var entry in config.entries)
        {
            try
            {
                Debug.Log($"[CSV] Download: {entry.name} - {entry.url}");


                // Google Sheet CSV 다운로드
                string csv = DownloadCSV(entry.url);

                if (entry.type == "CSV_ONLY")
                {
                    string savePath = $"Assets/_Proj/Data/CSV/{entry.name}.csv";
                    File.WriteAllText(savePath, csv);
                    Debug.Log($"[CSV] Saved CSV_ONLY: {savePath}");
                    continue;
                }

                // CSV 파싱 → ScriptableObject 생성
                ImportAllTables(entry.name, entry.type, csv);

                Debug.Log($"[CSV] Imported: {entry.name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[CSV] Error importing {entry.name}: {e.Message}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private string DownloadCSV(string url)
    {
        using (WebClient wc = new WebClient())
        {
            return wc.DownloadString(url);
        }
    }

    void ImportAllTables(string name, string type, string csv)
    {
        if (type != "SO")
        {
            Debug.LogWarning($"[CSV] {name} 은 type SO가 아니므로 건너뜀");
            return;
        }

        // JsonUtility로 메타 읽기
        string metaJson = File.ReadAllText(metaFilePath);
        //텍스트 파일을 열고, 파일의 모든 텍스트를 읽은 다음에 파일을 닫고
        var metaList = JsonUtility.FromJson<TableMetaList>(metaJson);
        //entries json으로 변환

        //type이 "SO"라면 테이블 이름별 전용 Parser 호출
        switch (name)
        {
            //CSV의 type이 "SO"인 경우가 추가될 때마다 case를 추가, 각 CSV에 맞는 Parser클래스 생성
            case "tbl_animal_mst":
                AnimalParser.Import(csv);
                break;
            case "tbl_artifact_mst":
                ArtifactParser.Import(csv);
                break;
            case "tbl_background_mst":
                BackgroundParser.Import(csv);
                break;
            case "tbl_chapter_mst":
                ChapterParser.Import(csv);
                break;
            case "tbl_codex_mst":
                CodexParser.Import(csv);
                break;
            case "tbl_costume_mst":
                CostumeParser.Import(csv);
                break;
            case "tbl_deco_mst":
                DecoParser.Import(csv);
                break;
            case "tbl_dialogue_mst":
                DialogueParser.Import(csv);
                break;
            case "tbl_dialogue_speakers_dtl":
                SpeakerParser.Import(csv);
                break;
            case "tbl_goods_mst":
                GoodsParser.Import(csv);
                break;
            case "tbl_home_mst":
                HomeParser.Import(csv);
                break;
            case "tbl_manual_mst":
                ManualParser.Import(csv);
                break;
            case "tbl_profile_icon_mst":
                Profile_iconParser.Import(csv);
                break;
            case "tbl_quest_mst":
                QuestParser.Import(csv);
                break;
            case "tbl_shop_item_dtl":
                Shop_itemParser.Import(csv);
                break;
            case "tbl_shop_mst":
                ShopParser.Import(csv);
                break;
            case "tbl_stage_mst":
                StageParser.Import(csv);
                break;
            case "tbl_treasure_mst":
                TreasureParser.Import(csv);
                break;
            default:
                Debug.LogWarning($"[CSV] {name} 변환 로직 없음");
                break;
        }
    }
}