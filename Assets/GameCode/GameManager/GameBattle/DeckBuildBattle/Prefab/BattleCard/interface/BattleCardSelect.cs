using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BattleCardSelect
{
    public List<ISelectCondition> conditions = new();

    public BattleCardSelect(BattleCardSelectInfo cardSelectInfo)
    {
        
    }

    public bool IsTrue(IBattleUnit battleUnit)
    {
        foreach (var condition in conditions)
        {
            if (!condition.Check(battleUnit)) return false;
        }
        return true;
    }
}

