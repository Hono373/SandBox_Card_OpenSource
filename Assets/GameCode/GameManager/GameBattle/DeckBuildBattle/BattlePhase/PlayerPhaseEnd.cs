using System;
using DG.Tweening; 
using Newtonsoft.Json;

public class PlayerPhaseEnd : BattlePhase
{
    [JsonIgnore] public OnPhaseStartFlow onPhaseStartFlow = new();
    [JsonIgnore] public NextMoveDecideFlow nextMoveDecideFlow = new();
    [JsonIgnore] public RoundEndDiscardFlow roundEndDiscardFlow = new();
    /// <summary>常驻能力牌里 timingEnum == PlayerPhaseEnd 的效果。</summary>
    [JsonIgnore] public CardPhaseTimingFlow cardPhaseTimingFlow = new();
    public override BattlePhase NextPhase => new EnemyPhaseStart();
    public override BattlePhase Start()
    {
        DeckBuildBattle.Active.phase.current = this;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        flow.Join(roundEndDiscardFlow.Start().flow);
        flow.Append(cardPhaseTimingFlow.Start(TimingEnum.PlayerPhaseEnd, needSettle: true).flow);
        flow.Join(onPhaseStartFlow.Start(this, needSettle: true).flow);
        flow.Append(nextMoveDecideFlow.Start(this, needSettle: true).flow);
    }
}