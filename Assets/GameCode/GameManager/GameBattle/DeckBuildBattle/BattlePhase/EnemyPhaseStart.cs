using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;

public class EnemyPhaseStart : BattlePhase
{
    [JsonIgnore] public OnPhaseStartFlow onPhaseStartFlow = new();
    [JsonIgnore] public RefreshUnitShieldFlowGroup shieldRefreshGroup = new();
    [JsonIgnore] public PhaseAdvanceFlow phaseAdvanceFlow = new();
    public override BattlePhase NextPhase => new EnemyPhase();
    public override BattlePhase Start()
    {
        DeckBuildBattle.Active.phase.current = this;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var enemy = DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Enemy);
        flow.Join(onPhaseStartFlow.Start(this, needSettle: true).flow);
        flow.Append(shieldRefreshGroup.Start(enemy, needSettle: true).flow);
        flow.Append(phaseAdvanceFlow.Start(NextPhase).flow);
    }
}
