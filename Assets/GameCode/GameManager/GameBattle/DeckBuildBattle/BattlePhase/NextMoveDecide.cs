using Newtonsoft.Json;

public class NextMoveDecide : GameModify
{
    public BattlePhase phase;
    public SortedWeight<BattlePhase> nextPhaseWeight = new();
    /// <summary>决定出的下一阶段，由 NextMoveDecideFlow 启动；运行时结果，不进存档。</summary>
    [JsonIgnore] public BattlePhase phaseResult;
    public NextMoveDecide Start(BattlePhase phase, bool needSettle)
    {
        this.phase = phase;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        phaseResult = nextPhaseWeight.MaxOrDefault(phase.NextPhase);
    }
}