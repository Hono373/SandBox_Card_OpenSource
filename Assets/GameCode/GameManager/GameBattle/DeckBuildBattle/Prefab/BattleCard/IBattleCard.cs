using DG.Tweening;

public interface IBattleCard : IInfiniteData
{
    public BattleCardData GetData();

    BattleCardUI GetObj();
    BattleCardUI InstantiateObj();
    void RecycleObj();
}
