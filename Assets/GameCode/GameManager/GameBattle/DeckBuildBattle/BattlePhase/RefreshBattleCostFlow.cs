using DG.Tweening;
using Newtonsoft.Json;

public class RefreshBattleCostFlow : BattleFlow
{
    public bool needSettle;
    public RefreshBattleCost refreshBattleCost = new();
    public RefreshBattleCardGroupFlow refreshBattleCardGroupFlow = new();
    public RefreshBattleCostFlow Start(bool needSettle)
    {
        this.needSettle = needSettle;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var data = DeckBuildBattle.Active.data;
        refreshBattleCost.Start(needSettle);
        data.energy = data.maxEnergy = refreshBattleCost.maxCostResult;
        var shot = DeckBuildBattle.Active.data.maxEnergy;

        flow.Append(refreshBattleCardGroupFlow.Start(needSettle).flow);
        var ui = DeckBuildBattle.GetUI().uiContainer.costContainer;
        flow.onComplete += () => ui.uiGroup.Alpha(true);
        flow.onComplete += () => ui.cost.text = $"{shot}/{shot}";
    }
}
