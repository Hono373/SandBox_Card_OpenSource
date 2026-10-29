using System;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
public class PlayerPhaseStart : BattlePhase
{
    [JsonIgnore] public OnPhaseStartFlow onPhaseStartStep = new();
    [JsonIgnore] public RefreshBattleCostFlow refreshBattleCostFlow = new();
    [JsonIgnore] public RefreshUnitShieldFlowGroup refreshUnitShieldFlows = new();
    [JsonIgnore] public DrawCardFlow drawCardFlow = new();
    [JsonIgnore] public PhaseAdvanceFlow phaseAdvanceFlow = new();
    /// <summary>常驻能力牌里 timingEnum == PlayerPhaseStart 的效果。</summary>
    [JsonIgnore] public CardPhaseTimingFlow cardPhaseTimingFlow = new();
    public override BattlePhase NextPhase => new PlayerPhase();
    public override BattlePhase Start()
    {
        DeckBuildBattle.Active.phase.current = this;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var mate = DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Mate);
        flow.Join(onPhaseStartStep.Start(this, needSettle: true).flow);
        flow.Append(refreshBattleCostFlow.Start(needSettle: true).flow);
        flow.Join(refreshUnitShieldFlows.Start(mate, needSettle: true).flow);
        // 放在清格挡之后：回合开始获得格挡的能力牌不应该被刷新流程抹掉
        flow.Append(cardPhaseTimingFlow.Start(TimingEnum.PlayerPhaseStart, needSettle: true).flow);
        flow.Join(drawCardFlow.Start(DeckBuildBattle.GetRule().drawNumb, isSkillEffect: false, needSettle: true).flow);
        flow.Append(phaseAdvanceFlow.Start(NextPhase).flow);
    }

}