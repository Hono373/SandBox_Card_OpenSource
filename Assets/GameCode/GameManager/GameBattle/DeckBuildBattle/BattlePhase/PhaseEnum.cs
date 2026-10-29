/// <summary>战斗阶段时机，用于筛选 onPhase 触发点</summary>
public enum PhaseEnum
{
    Any,
    BattleStart,
    PlayerPhaseStart,
    PlayerPhaseEnd,
    EnemyPhaseStart,
    EnemyPhaseEnd,
    Death,
}
