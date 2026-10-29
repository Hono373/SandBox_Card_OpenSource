using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConditionBox
{
    [InfoBox("外部重写IGameCondition")]
    [SerializeReference] List<IGameCondition_Ob> rules = new();
    public int? Result(BattleUnit unit)
    {
        foreach (var rule in rules)
        {
            rule.Condition(unit, out var result);
            if (result.HasValue)
                return result.Value;
        }
        return null;
    }
}
