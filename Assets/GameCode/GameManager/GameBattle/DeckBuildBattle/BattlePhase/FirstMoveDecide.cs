using System;
using Newtonsoft.Json;

/// <summary>
/// 战斗开始时判定先手：默认进入 PlayerPhaseStart，
/// 被偷袭等规则由 Modifier 往 nextPhaseWeight 投入 EnemyPhaseStart 改写。
/// 只用于 BattlePhaseStart，与阶段间推进（NextPhase）是两回事。
///
/// 这里不再做 isGameOver 早退：本类只产出结果、不直接改 current，
/// 拦截交给 PhaseAdvanceFlow 走 BattleFlow.Init 的单点闸门。
/// </summary>
[Serializable]
public class FirstMoveDecide : GameModify
{
    public BattlePhaseStart phase;
    public SortedWeight<BattlePhase> nextPhaseWeight = new();
    /// <summary>决定出的首个行动阶段，由 PhaseAdvanceFlow 启动。</summary>
    [JsonIgnore] public BattlePhase firstPhase;

    public FirstMoveDecide Start(BattlePhaseStart phase, bool needSettle)
    {
        this.phase = phase;
        ModifyAndCreateResult(needSettle);
        return this;
    }

    protected override void CreateResult()
    {
        firstPhase = nextPhaseWeight.MaxOrDefault(phase.NextPhase);
    }
}
