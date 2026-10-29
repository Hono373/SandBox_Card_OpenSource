using System;
using UnityEngine;

[Serializable]
public class CostNo : BattleCardCostRule
{
    public CostNo() { }
    public static CostNo Clone(CostNo info) => new CostNo();
    public override int Cost() => 0;
    public override BattleCardCostUIData RefreshCostUI() => new(false, string.Empty);
    public override int GetResult()=>0;
    public override BattleCardCostRule Create() => Clone(this);
    public override int Apply(int modify)=> 0;
}
