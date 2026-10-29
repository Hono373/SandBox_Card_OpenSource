using System.Collections.Generic;
using DG.Tweening;
public class UnitActionPhaseGroupFlow : BattleFlow
{
    List<BattleUnit> units;
    public List<UnitActionFlow> unitActionFlows = new();
    public UnitActionPhaseGroupFlow Start(List<BattleUnit> units)
    {
        this.units = units;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        foreach (var unit in units)
        {
            var unitActionFlow = new UnitActionFlow().Start(unit);
            unitActionFlows.Add(unitActionFlow);
            flow.Append(unitActionFlow.flow);
        }
    }
}
