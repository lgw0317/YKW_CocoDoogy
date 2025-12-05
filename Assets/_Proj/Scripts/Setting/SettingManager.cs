using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SettingData // 게임 설정처럼 옵션 저장하고 싶으면 데이터 넣으세요
{
    public AudioVolumeData audio = new AudioVolumeData();
    public List<AnimalPositionEntry> animalPos = new List<AnimalPositionEntry>();
}

//[DefaultExecutionOrder(-100)]
public class SettingManager : MonoBehaviour
{
    public SettingData settingData;
    public static SettingManager Instance { get; private set; }

    private string savePath;
    // 플랫폼 분할 윈도우와 나머지들. 윈도우 저장 경로 내문서->코코두기->option.json

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
#if UNITY_STANDALONE_WIN
    savePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "CocoDoogy", "option.json");
#else
    savePath = Path.Combine(Application.persistentDataPath, "options.json");
#endif

        LoadSettings();
    }

#region 오디오
    public void SaveSettings()
    {
        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        string json = JsonUtility.ToJson(settingData);
        File.WriteAllText(savePath, json);
    }

    public void LoadSettings()
    {
        if (!File.Exists(savePath))
        {
            // 파일이 없을 시 새로 생성
            settingData = new SettingData { audio = new AudioVolumeData() };
            SaveSettings();
            return;
        }

        string json = File.ReadAllText(savePath);
        settingData = JsonUtility.FromJson<SettingData>(json);
    }
#endregion

#region 로비동물캐릭터 위치

    /// <summary>
    /// 로비에 있는 동물 오브젝트의 위치 값 지정
    /// </summary>
    /// <param name="name">로비에 있는 동물 오브젝트 이름</param>
    /// <param name="pos">해당 동물 오브젝트의 위치 값</param>
    public void SetAnimalPosition(string name, Vector3 pos)
    {
        var lobbyManager = LobbyCharacterManager.Instance;
        if (lobbyManager == null) return;

        var entry = settingData.animalPos.Find(x => x.objectName == name);
        if (entry == null)
        {
            settingData.animalPos.Add(new AnimalPositionEntry
            {
                objectName = name,
                objectPos = pos
            });
        }
        else
        {
            entry.objectPos = pos;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">로비에 있는 동물 오브젝트 이름</param>
    /// <param name="pos">해당 동물 오브젝트의 위치 값</param>
    /// <returns></returns>
    public bool TryGetAnimalPosition(string name, out Vector3 pos)
    {
        var entry = settingData.animalPos.Find(x => x.objectName == name);
        if (entry != null)
        {
            pos = entry.objectPos;
            return true;
        }
        pos = LobbyCharacterManager.Instance.Waypoints[0].transform.position;
        return false;
    }
    /// <summary>
    /// 로비에 있는 동물 오브젝트 삭제 시 데이터 삭제
    /// </summary>
    /// <param name="name">로비에 있는 동물 오브젝트 이름</param>
    public void RemoveAnimalPosition(string name)
    {
        settingData.animalPos.RemoveAll(x => x.objectName == name);
    }

    public void RefreshAnimalPositionEntryList()
    {
        var manager = LobbyCharacterManager.Instance;
        if (manager == null) return;

        var lobbyCharacters = LobbyCharacterManager.Instance.LobbyCharacter;

        HashSet<string> aliveInLobbyScene = new HashSet<string>();

        // 로비에 없는 애들 삭제
        foreach (var lC in lobbyCharacters)
        {
            if (lC is BaseLobbyCharacterBehaviour mono)
            {
                if (mono != null && mono.isActiveAndEnabled && mono.CompareTag("Animal"))
                {
                    aliveInLobbyScene.Add(mono.name);
                }
            }
        }
        settingData.animalPos.RemoveAll(x => !aliveInLobbyScene.Contains(x.objectName));
        // 로비에 있는 애들 등록
        foreach (var lC in lobbyCharacters)
        {
            if (lC is BaseLobbyCharacterBehaviour mono)
            {
                if (mono != null && mono.isActiveAndEnabled && mono.CompareTag("Animal"))
                {
                    SetAnimalPosition(mono.name, mono.transform.position);
                }
            }
        }

        SaveSettings();
    }

    #endregion

    private void OnApplicationPause(bool pause)
    {
        if (!pause) return;

        var manager = LobbyCharacterManager.Instance;
        if (manager == null) return;

        var lobbyCharacters = LobbyCharacterManager.Instance.LobbyCharacter;

        foreach (var lC in lobbyCharacters)
        {
            if (lC is BaseLobbyCharacterBehaviour mono)
            {
                if (mono != null && mono.isActiveAndEnabled && mono.CompareTag("Animal"))
                {
                    SetAnimalPosition(mono.name, mono.transform.position);
                }
            }
        }
        SaveSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

}
