using System;

/// <summary>
/// 条件：历史上累计出现指定类型**未满** count 次时通过，满了之后永久剔除本节点。
/// 与 CumulativeType 互为反面，成对使用才能表达「蓄力 N 次 → 之后一直用别的招」。
///
/// 2026-10-23 新增，配合 CumulativeType。注意它和 NotConsecutiveType 的区别：
/// NotConsecutiveType 看最近 N 次，一旦被别的类型打断就会重新通过；
/// 本类看累计总数，达标后不会再退回去。
/// </summary>
[Serializable]
public class NotCumulativeType : Intention_Condition
{
    public EffectTypeEnum type;
    public int count = 1;

    public override bool Check(BattleUnit unit)
    {
        var history = unit.skillHistory;
        var total = 0;
        for (var i = 0; i < history.Count; i++)
        {
            if (history[i].effectTypeEnum == type)
                total++;
        }
        return total < count;
    }

    public override string ToString() => $"累计{type}未满{count}次才可选";
}
