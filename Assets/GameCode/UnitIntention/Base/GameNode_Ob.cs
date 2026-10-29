using System;
using System.Collections.Generic;

/// <summary>
/// 条件权重混合节点
/// </summary>
public abstract class GameNode_Ob : IGameNode_Ob
{
    public virtual bool IsEnd => false;
    public abstract int Weight { get; }
    public virtual IReadOnlyList<int> ChildsWeights()
    {
        List<int> list = new();
        foreach (var child in Childs)
        {
            list.Add(child.Weight);
        }
        return list;
    }
    public abstract ConditionBox ConditionBox { get; }
    public abstract IReadOnlyList<IGameNode_Ob> Childs { get; }
    public int? GetChildIndex(BattleUnit unit)
    {
        var result = ConditionBox.Result(unit);
        if (!result.HasValue)
        {
            var weights = ChildsWeights();
            if (weights?.Count == 0)
                return null;
            result = new WeightedRandomSelector(weights).SelectIndex();
        }
        return result.Value;
    }
}