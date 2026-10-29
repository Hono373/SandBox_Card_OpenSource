using System;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
[Serializable]
public class BattlePhaseStart : BattlePhase
{
    [JsonIgnore] public BattleStartSkillFlow battleStartSkillFlow = new();
    [JsonIgnore] public OnPhaseStartFlow onPhaseStartFlow = new();
    [JsonIgnore] public FirstMoveDecide firstMoveDecide = new();
    [JsonIgnore] public PhaseAdvanceFlow phaseAdvanceFlow = new();

    [JsonIgnore] public RefreshUnitIntentionGroupFlow refreshUnitIntentionGroupFlow = new();
    public override BattlePhase NextPhase => new PlayerPhaseStart();
    public override BattlePhase Start()
    {
        GameSerialize.Save();
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var all = DeckBuildBattle.Active.data.unitDict.GetAll().Values;

        flow.Append(battleStartSkillFlow.Start(all, needSettle: true).flow);
        flow.Append(onPhaseStartFlow.Start(this, needSettle: true).flow);
        flow.Join(refreshUnitIntentionGroupFlow.Start(all, needSettle: true).flow);
        // 先手判定后等遮罩退场再推进，其余阶段用 phaseAdvanceFlow.Start(NextPhase)
        firstMoveDecide.Start(this, needSettle: true);
        flow.Append(phaseAdvanceFlow.StartAfterLoading(firstMoveDecide.firstPhase).flow);
    }
}
