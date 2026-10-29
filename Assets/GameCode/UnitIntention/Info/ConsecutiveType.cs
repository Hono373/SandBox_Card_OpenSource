using System;

/// <summary>
/// 条件：最近 count 次行动全都是指定类型时才通过（与 NotConsecutiveType 相反）。
/// 用于"必须先蓄力/充能 N 次才能放大招"这类规则。
/// </summary>
[Serializable]
public class ConsecutiveType : Intention_Condition
{
    /// <summary>要比对的行动类型。</summary>
    public EffectTypeEnum type;
    /// <summary>需要连续满足的次数，历史条数不足该值时一律不通过。</summary>
    public int count = 1;

    public override bool Check(BattleUnit unit)
    {
        var history = unit.skillHistory;
        if (history.Count < count) return false;
        for (var i = history.Count - count; i < history.Count; i++)
        {
            if (history[i].effectTypeEnum != type) return false;
        }
        return true;
    }

    public override string ToString() => $"最近{count}次都是{type}才可选";
}
