using DG.Tweening;

public class AddCostFlow : BattleFlow
{
    public AddCostEffect addCost;
    public BattleUnit caster;
    public BattleUnit target;
    public AddCostFlow Start(AddCostEffect addCost, BattleUnit caster, BattleUnit target)
    {
        this.addCost = addCost;
        this.caster = caster;
        this.target = target;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var data = DeckBuildBattle.Active.data;
        flow.Join(DeckBuildBattle.GetUI().uiContainer.RefreshCostUI(data.energy, data.maxEnergy));
    }
}
