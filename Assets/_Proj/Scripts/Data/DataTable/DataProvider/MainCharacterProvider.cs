using UnityEngine;

public class MainCharacterProvider : IDataProvider<int, MainCharacterData>
{
    // 
    private MainCharacterDatabase database;
    private IResourceLoader loader;

    public MainCharacterProvider(MainCharacterDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public MainCharacterData GetData(int id)
    {
        return database.mainCharDataList.Find(a => a.mainChar_id == id);
    }

    public GameObject GetPrefab(int id)
    {
        var data = GetData(id);
        return data?.GetPrefab(loader);
    }
    public Sprite GetIcon(int id)
    {
        var data = GetData(id);
        return data?.GetIcon(loader);
    }
}
