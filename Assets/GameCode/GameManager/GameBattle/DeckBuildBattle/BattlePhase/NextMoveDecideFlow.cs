using Newtonsoft.Json;

public class NextMoveDecideFlow : BattleFlow
{
    public bool needSettle;
    public BattlePhase phase;
    public BattlePhase phaseResult;
    [JsonIgnore] public NextMoveDecide nextMoveDecide = new();
    public NextMoveDecideFlow Start(BattlePhase phase, bool needSettle)
    {
        this.phase = phase;
        this.needSettle = needSettle;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        nextMoveDecide.Start(phase, needSettle);
        phaseResult = nextMoveDecide.phaseResult;
        flow.onComplete += () => phaseResult.Start();
    }
}
