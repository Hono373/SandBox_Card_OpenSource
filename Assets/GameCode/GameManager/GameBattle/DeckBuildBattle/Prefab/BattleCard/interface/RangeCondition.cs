using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class RangeCondition
{
    public abstract List<BattleUnit> Done(List<BattleUnit> list);
    internal abstract RangeCondition Create();

    internal virtual void ValidateData() { }
}
