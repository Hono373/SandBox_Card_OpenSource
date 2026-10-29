using System;

/// <summary>
/// 条件：自己阵营存活单位数量（含自己）满足比较时通过。
/// 用于"有队友才放辅助招、独自一人时改用攻击"这类规则。
/// </summary>
[Serializable]
public class MateAliveCount : Intention_Condition
{
    /// <summary>true = 存活数 ≥ count；false = 存活数 ≤ count。</summary>
    public bool atLeast;
    /// <summary>比较的阈值（含自己）。</summary>
    public int count = 1;

    public override bool Check(BattleUnit unit)
    {
        var mates = DeckBuildBattle.Active.data.unitDict.GetByCamp(unit.camp.camp);
        var alive = 0;
        foreach (var mate in mates)
        {
            if (mate.current.isDead || mate.current.nearDeath || mate.data.hp <= 0) continue;
            alive++;
        }
        return atLeast ? alive >= count : alive <= count;
    }

    public override string ToString() => $"己方存活{(atLeast ? "≥" : "≤")}{count}";
}
