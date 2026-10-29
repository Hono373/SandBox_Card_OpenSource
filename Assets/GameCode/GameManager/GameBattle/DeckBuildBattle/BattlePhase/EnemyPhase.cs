using System;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;

public class EnemyPhase : BattlePhase
{
    [JsonIgnore] public UnitActionPhaseGroupFlow unitActionPhaseGroupFlow = new();
    [JsonIgnore] public PhaseAdvanceFlow phaseAdvanceFlow = new();
    public override BattlePhase NextPhase => new EnemyPhaseEnd();
    public override BattlePhase Start()
    {
        DeckBuildBattle.Active.phase.current = this;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var enemy = DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Enemy);
        flow.Append(unitActionPhaseGroupFlow.Start(enemy).flow);
        flow.Append(phaseAdvanceFlow.Start(NextPhase).flow);
    }
}
