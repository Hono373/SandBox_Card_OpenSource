using DG.Tweening;

public class OnPhaseStartFlow : BattleFlow
{
    public bool needSettle;
    public BattlePhase phase;
    public OnPhaseStartModify onPhaseStartModify = new();
    public RefreshBattleCardGroupFlow refreshBattleCardGroupFlow = new();
    public BattleEndCheckFlow battleEndCheckFlow = new();
    public OnPhaseStartFlow Start(BattlePhase phase, bool needSettle)
    {
        this.needSettle = needSettle;
        this.phase = phase;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        onPhaseStartModify.Start(phase, needSettle);
        foreach (var skillApplyFlow in onPhaseStartModify.skillApplyFlows)
        {
            flow.Append(skillApplyFlow.flow);
        }
        flow.Append(refreshBattleCardGroupFlow.Start(needSettle: false).flow);
        flow.Append(battleEndCheckFlow.Start().flow);
    }
}
