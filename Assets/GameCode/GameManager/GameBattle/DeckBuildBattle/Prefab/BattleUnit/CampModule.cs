using System;
using System.Collections.Generic;

[Serializable]
public class CampModule
{
    public CampEnum camp;
    CampModule() { }
    public CampModule(CampEnum campEnum) { camp = campEnum; }
    public List<BattleUnit> GetRivalList()
    {
        var targetList = new List<BattleUnit>();
        switch (camp)
        {
            case CampEnum.Enemy:
                targetList.AddRange(DeckBuildBattle.Active.data.GetPlayer());
                break;

            case CampEnum.Mate:
                targetList.AddRange(DeckBuildBattle.Active.data.GetEnemyList());
                break;
        }
        return targetList;
    }
}