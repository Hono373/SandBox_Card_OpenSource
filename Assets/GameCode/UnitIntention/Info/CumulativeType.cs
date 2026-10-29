using System;

/// <summary>
/// 条件：历史上累计出现过 count 次指定类型后**永久通过**。
///
/// 2026-10-23 新增。地精法师要的是「？？？ ？？？ 攻击 攻击 攻击…」——
/// 蓄满之后一直攻击，不再回去蓄力。原先只有 ConsecutiveType（最近 N 次都是）做不到：
/// 它要求「最近两次都是 ？？？」，攻击一次之后条件立刻失效，会被打回蓄力，
/// 变成「？？？ ？？？ 攻击」无限循环。区别在于 Consecutive 看**最近**，Cumulative 看**累计**，
/// 且累计一旦达标就不会再退回去。
/// </summary>
[Serializable]
public class CumulativeType : Intention_Condition
{
    /// <summary>要比对的行动类型。</summary>
    public EffectTypeEnum type;
    /// <summary>累计出现多少次之后开始通过。历史不足时一律不通过。</summary>
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
        return total >= count;
    }

    public override string ToString() => $"累计{type}满{count}次后恒可选";
}
