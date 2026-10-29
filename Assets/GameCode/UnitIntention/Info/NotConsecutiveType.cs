using System;

/// <summary>条件：最近 count 次行动连续都是指定类型时剔除本节点（用于"连续两次攻击后必须换招"这类规则）。</summary>
[Serializable]
public class NotConsecutiveType : Intention_Condition
{
    public EffectTypeEnum type;
    public int count = 1;
    public override bool Check(BattleUnit unit)
    {
        var history = unit.skillHistory;
        if (history.Count < count) return true;
        for (var i = history.Count - count; i < history.Count; i++)
        {
            if (history[i].effectTypeEnum != type) return true;
        }
        return false;
    }
    public override string ToString() => $"{type} 连续{count}次则剔除";
}
