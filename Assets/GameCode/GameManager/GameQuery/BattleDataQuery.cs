using System.Collections.Generic;

public static class BattleDataQuery
{

    internal static List<BattleUnit> GetPlayer(this DeckBuildBattleData data) => data.unitDict[CampEnum.Mate];

    internal static List<BattleUnit> GetEnemyList(this DeckBuildBattleData data) => data.unitDict[CampEnum.Enemy];
}
