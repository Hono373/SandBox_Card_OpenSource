using System;
using UnityEngine;

[Serializable]
public class CostValue : BattleCardCostRule
{
    public int cost = 0;
    public CostValue() { }
    public override int Cost() => cost;
    public override int Apply(int modify) => (cost + modify).GreaterThanOrEqual(0);
    public override BattleCardCostUIData RefreshCostUI() => new(true, cost.ToString());
    public override int GetResult()
    {
        return cost;
    }
    public override BattleCardCostRule Create() => new CostValue { cost = cost };
}