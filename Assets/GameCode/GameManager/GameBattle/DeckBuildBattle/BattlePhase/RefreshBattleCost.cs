public class RefreshBattleCost : GameModify
{
    public int maxCostModifi;
    public int maxCostResult;
    public RefreshBattleCost Start(bool needSettle)
    {
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {

        maxCostResult = (DeckBuildBattle.GetRule().maxCost + maxCostModifi).GreaterThanOrEqual();
      
    }
}
