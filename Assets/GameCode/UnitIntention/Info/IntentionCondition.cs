using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class IntentionCondition
{
    [SerializeField][SerializeReference] List<Intention_Condition> rules = new();
    public int Count => rules.Count;
    public IReadOnlyList<Intention_Condition> Rules => rules;
    /// <summary>所有规则都通过才算通过；rules 为空视为无限制。</summary>
    public bool Result(BattleUnit unit)
    {
        foreach (var rule in rules)
        {
            if (!rule.Check(unit)) return false;
        }
        return true;
    }
}
