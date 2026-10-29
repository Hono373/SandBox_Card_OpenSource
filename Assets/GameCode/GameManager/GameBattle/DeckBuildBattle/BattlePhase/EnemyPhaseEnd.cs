using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;

public class EnemyPhaseEnd : BattlePhase
{
    [JsonIgnore] public OnPhaseStartFlow onPhaseStartFlow = new();
    [JsonIgnore] public PhaseAdvanceFlow phaseAdvanceFlow = new();
    public override BattlePhase NextPhase => new PlayerPhaseStart();
    public override BattlePhase Start()
    {
        DeckBuildBattle.Active.phase.current = this;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        flow.Join(onPhaseStartFlow.Start(this, needSettle: true).flow);
        flow.Append(phaseAdvanceFlow.Start(NextPhase).flow);
    }
}
