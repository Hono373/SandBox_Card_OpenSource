using System;
using System.Collections.Generic;

[Serializable]
public class DeckBuildBattleData
{
    public string key;
    public Winner winner = Winner.None;
    public BattleUnitDict unitDict;

    public int roundNumb = 0;
    public int energy = 0;
    public int maxEnergy = 0;
    DeckBuildBattleData() { }
    public static DeckBuildBattleData Create(GameData active, DeckBuildBattleInfo info)
    {
        var instance = new DeckBuildBattleData();
        instance.key = info.key;

        instance.unitDict = new BattleUnitDict();

        var unitLib = active.unitLibrary;

        {
            var gameInfo = GameInfo.Get();
            var playerUnit = unitLib.GetUnitById(gameInfo.playerKey);
            var playerBattleData = BattleUnit.Create(playerUnit, CampEnum.Mate);
            instance.unitDict.Add(playerBattleData.camp.camp, playerBattleData);

            foreach (var enemy in info.enemys)
            {
                Unit unit;
                if (unitLib.ContainsId(enemy.key))
                {
                    unit = unitLib.GetUnitById(enemy.key);
                }
                else
                {
                    unit = unitLib.CreateUnit(enemy.key, true);
                }
                var enemyBattleData = BattleUnit.Create(unit, CampEnum.Enemy);
                instance.unitDict.Add(enemyBattleData.camp.camp, enemyBattleData);
            }
        }
        return instance;
    }
}
