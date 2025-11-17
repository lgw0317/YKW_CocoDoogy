using UnityEngine;
using static UserData;

// UserData.Local.wallet을 사용하는 IGoodsStore 구현체
// 에너지/캡/코인을 UserData 기준으로 관리
public class UserDataGoodsStore : IGoodsStore
{
    private readonly int _energyId;
    private readonly int _capId;
    private readonly int _coinId;

    public UserDataGoodsStore(int energyId, int capId, int coinId)
    {
        _energyId = energyId;
        _capId = capId;
        _coinId = coinId;
    }

    private Wallet GetWallet()
    {
        if (UserData.Local == null)
        {
            Debug.LogWarning("[UserDataGoodsStore] UserData.Local 이 null ");
            return null;
        }

        if (UserData.Local.wallet == null)
            UserData.Local.wallet = new Wallet();

        return UserData.Local.wallet;
    }

    public int GetAmount(int goodsId)
    {
        var wallet = GetWallet();
        if (wallet == null) return 0;

        if (goodsId == _energyId) return wallet.energy;
        if (goodsId == _capId) return wallet.cap;
        if (goodsId == _coinId) return wallet.coin;

        Debug.LogWarning($"[UserDataGoodsStore] 알 수 없는 goodsId: {goodsId}");
        return 0;
    }

    public void SetAmount(int goodsId, int amount)
    {
        var wallet = GetWallet();
        if (wallet == null) return;

        int v = Mathf.Max(0, amount);

        if (goodsId == _energyId)
            wallet.energy = v;
        else if (goodsId == _capId)
            wallet.cap = v;
        else if (goodsId == _coinId)
            wallet.coin = v;
        else
        {
            Debug.LogWarning($"Unknown goodsId {goodsId}");
            return;
        }

        UserData.Local.flag |= UserDataDirtyFlag.Wallet;
        wallet.Save();
    }
}
