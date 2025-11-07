using UnityEngine;

public class GoodsProvider : IDataProvider<int, GoodsData>
{
    //각 데이터 별로 Provider 클래스를 두어서 자기 데이터만 관리하게 함
    private GoodsDatabase database;
    private IResourceLoader loader;

    public GoodsProvider(GoodsDatabase db, IResourceLoader resLoader)
    {
        database = db;
        loader = resLoader;
    }

    public GoodsData GetData(int id)
    {
        return database.goodsList.Find(a => a.goods_id == id);
    }

    public Sprite GetIcon(int id)
    {
        var data = GetData(id);
        return data?.GetIcon(loader);
    }
}