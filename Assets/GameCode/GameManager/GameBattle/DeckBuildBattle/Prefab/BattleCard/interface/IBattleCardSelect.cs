using System.Collections.Generic;

/// <summary>
/// 目标游戏对象接口
/// </summary>
public interface IBattleCardSelect
{
    CampEnum BaseTeam { get; }
    CampEnum Team(BattleCard card);
    bool IsLegalTarget(BattleCard card, BattleUnit target);
}
