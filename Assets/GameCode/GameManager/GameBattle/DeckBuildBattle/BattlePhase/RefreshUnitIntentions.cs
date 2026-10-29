using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class RefreshUnitIntentions
{
    public List<BattleUnit> units = new();
    public RefreshUnitIntentions Start(ICollection<BattleUnit> units)
    {
        this.units.AddRange(units);
        foreach (var unit in units)
        {
            var intentionCurrent = unit.Info.intention.Create();
            unit.unitSkill = intentionCurrent.GetResult(unit);
        }
        return this;
    }
}
