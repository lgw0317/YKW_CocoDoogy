using System;
using UnityEngine;

public class GoodsService : IQuestBehaviour
{
    private readonly IGoodsStore store;
    public Action onValueChanged;
    public GoodsService(IGoodsStore store)
    {
        this.store = store;
        //UserData.Local.goods.onValueChanged += onValueChanged;
    }

    public int Get(int goodsId)
    {
        return store.GetAmount(goodsId);
    }

    public void Add(int goodsId, int amount)
    {
        int cur = store.GetAmount(goodsId);
        store.SetAmount(goodsId, cur + amount);

        //UI등 리셋 호출용
        onValueChanged?.Invoke();

        //퀘스트 핸들링: 병뚜껑 모으기
        if (goodsId == 110002)
            this.Handle(QuestObject.collect_cap, value: amount);

        //추가: 재화 타입의 해금 처리. 아 코드 또 이상하네 이거
        UserData.Local.codex[CodexType.artifact, goodsId] = true;
    }

    public bool TrySpend(int goodsId, int amount)
    {
        int cur = store.GetAmount(goodsId);
        if (cur < amount)
            return false;

        store.SetAmount(goodsId, cur - amount);
        
        //퀘스트 핸들링: 병뚜껑 쓰기
        if (goodsId == 110002)
            this.Handle(QuestObject.use_cap, value: -amount);

        onValueChanged?.Invoke();
        return true;
    }
}