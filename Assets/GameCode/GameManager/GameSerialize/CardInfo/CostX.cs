using System;
using UnityEngine;

[Serializable]
public class CostX : BattleCardCostRule
{
    public CostX() { }
    public override int Cost() => DeckBuildBattle.Active.data.energy;
    public override BattleCardCostUIData RefreshCostUI() => new(true, "X");
    public override int GetResult()
    {
        return DeckBuildBattle.Active.data.energy;
    }
    public override BattleCardCostRule Create() => new CostX();

    public override int Apply(int modify) => DeckBuildBattle.Active.data.energy;
}
