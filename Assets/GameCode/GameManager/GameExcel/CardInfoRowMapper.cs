using System;
using System.Collections.Generic;

/// <summary>
/// 把 CardInfo 实例摊平成 DB.xlsx（Card sheet）的「行集合」。
/// 每行是一个「列号 -> 单元格值」字典；没值的列不出现在字典里，
/// 调用方只写出现的列，未出现的列保持 Excel 原值不动。
///
/// 列号依据 Defines/Table.xml + DB.xlsx 的 ##var 表头布局硬编码（表头增删列需同步这里）。
///
/// 分行规则对齐 Luban 的 multi_rows 语义（见 RowColumnSheet.SplitRows / IsMultiRowsExtendRow）：
///   - 一张卡占多行；**第 i 行承载每个列表的第 i 个元素**，某列表在该行没有元素时其列留空（=不推进）
///   - timing：每个 timing 至少 1 行；timingEnum/pileEnum 只写在每个 timing 的第一行
///   - skill：技能主体只写该 skill 的第一行；skill 的 rangeRule.conditions 有 N 个就占 N 行
///     （第 k 行写第 k 个条件，其余行只带条件列）
///   - choiceRule.conditions：第 i 个条件写在整卡的**第 i 行**（行不够则在末尾补行）
///   - 卡级字段（cardName/cost/interactive/graphic/choiceRule.range）只写在第一行
/// Excel 侧靠「列全空 = 延续」来还原，因此留空的列必须真的留空。
/// </summary>
public static class CardInfoRowMapper
{
    public const int MaxColumn = 31;   // 表头末列（AE = ethereal；AD = unplayable）
                                      // 注：effect 子字段被 Luban 限制在 U~AC 内，新 effect 只能复用这 9 列

    public static bool TryToRows(CardInfo c, out List<Dictionary<int, object>> rows)
    {
        rows = new List<Dictionary<int, object>>();
        if (c == null) return false;

        var timings = c.timings ?? new List<BattleTiming>();

        if (timings.Count == 0)
        {
            rows.Add(new Dictionary<int, object>());
        }
        else
        {
            foreach (var t in timings)
            {
                var skills = t.skills ?? new List<Skill>();
                if (skills.Count == 0)
                {
                    // 该 timing 没有 skill：单独占一行，只带 timingEnum/pileEnum
                    var r = new Dictionary<int, object>();
                    PutTiming(r, t);
                    rows.Add(r);
                }
                else
                {
                    for (int i = 0; i < skills.Count; i++)
                    {
                        var skill = skills[i];
                        var conds = skill?.rangeRule?.conditions;
                        int condCount = conds?.Count ?? 0;
                        int rowCount = Math.Max(1, condCount);   // 有 N 个条件就占 N 行
                        for (int k = 0; k < rowCount; k++)
                        {
                            var r = new Dictionary<int, object>();
                            if (k == 0)
                            {
                                PutSkill(r, skill);              // 技能主体只写该 skill 的第一行
                                if (i == 0) PutTiming(r, t);     // timing 级字段只写该 timing 的第一行
                            }
                            if (k < condCount) PutRangeCondition(r, conds[k]);
                            rows.Add(r);
                        }
                    }
                }
            }
        }

        // choiceRule.conditions：第 i 个条件落在整卡第 i 行（行不够则补行）
        var choiceConds = c.choiceRule?.conditions;
        if (choiceConds != null)
        {
            for (int i = 0; i < choiceConds.Count; i++)
            {
                while (rows.Count <= i) rows.Add(new Dictionary<int, object>());
                PutChoiceCondition(rows[i], choiceConds[i]);
            }
        }

        // 卡级字段合并进第一行
        PutCardLevel(rows[0], c);
        return true;
    }

    static void PutCardLevel(Dictionary<int, object> r, CardInfo c)
    {
        Put(r, 3, c.cardName);

        // cost（多态）
        Put(r, 4, c.cost?.GetType().Name);
        if (c.cost is CostValue cv) Put(r, 5, cv.cost);

        // interactive（多态）
        Put(r, 6, c.interactive?.GetType().Name);

        // graphic
        if (c.graphic != null)
        {
            Put(r, 7, (int)c.graphic.color);
            Put(r, 8, (int)c.graphic.type);
            Put(r, 9, (int)c.graphic.rarity);
        }

        // choiceRule.range（条件单独按行写，见 TryToRows / PutChoiceCondition）
        if (c.choiceRule != null)
        {
            Put(r, 10, (int)c.choiceRule.range);
        }

        // unplayable / ethereal 只在 true 时写：Excel 留空即 false。
        // 若照常写 false，同步一次就会把整表 90 多行填满 FALSE，翻表时噪声太大。
        if (c.unplayable) Put(r, 30, true);
        if (c.ethereal) Put(r, 31, true);
    }

    static void PutTiming(Dictionary<int, object> r, BattleTiming t)
    {
        if (t == null) return;
        Put(r, 13, (int)t.timingEnum);
        Put(r, 14, (int)t.pileEnum);
    }

    static void PutSkill(Dictionary<int, object> r, Skill s)
    {
        if (s == null) return;
        if (s.rangeRule != null)
        {
            Put(r, 15, s.rangeRule.active);
            Put(r, 16, (int)s.rangeRule.baseRange);
        }

        Put(r, 19, s.countRule?.GetType().Name);
        if (s.countRule is FixedCount fc) Put(r, 20, fc.count);
        else if (s.countRule is CountXUseCost cx) Put(r, 20, cx.count);
        else if (s.countRule is VariableCount vc) Put(r, 20, vc.count);

        var e = s.effect;
        Put(r, 21, e?.GetType().Name);
        // hasSource 是 SkillEffect 基类字段，所有效果共有（紧跟 $type 之后）
        if (e != null) Put(r, 22, e.hasSource);
        // Damage / Heal / Shield / DrawCardEffect / AddCostEffect 都有 value，共用第 23 列（effect.value）；
        // 漏掉任何一个会导致它的 value 不被写回、被清成默认值 0。
        if (e is Damage dmg) Put(r, 23, dmg.value);
        else if (e is Heal heal) Put(r, 23, heal.value);
        else if (e is Shield shield) Put(r, 23, shield.value);
        else if (e is DrawCardEffect drawCardEffect) Put(r, 23, drawCardEffect.value);
        else if (e is AddCostEffect addCostEffect) Put(r, 23, addCostEffect.value);
        else if (e is ModifierAdd ma)
        {
            Put(r, 24, ma.key);
            Put(r, 25, ma.layer);
            Put(r, 26, ma.only);
        }
        // ExhaustHandEffect 的 value 是 TypeEnum 数值（0=None 1=Attack 2=Skill 3=Ability），不是数值量
        else if (e is ExhaustHandEffect exhaustHand) Put(r, 23, (int)exhaustHand.value);
        // 狂宴：伤害走 W(23)，击杀加成走 Y(25，与 ModifierAdd.layer 共用)
        else if (e is FeedEffect feed)
        {
            Put(r, 23, feed.value);
            Put(r, 25, feed.layer);
        }
        // 塞牌：数量走 W(23)、目标堆走 Y(25)、卡牌 key 走 X(24)
        else if (e is AddCardEffect addCard)
        {
            Put(r, 23, addCard.value);
            Put(r, 24, addCard.key);
            Put(r, 25, (int)addCard.layer);
        }
        // 巩固 / 坚毅 / 恶魔之焰：主参数都是 W(23)
        else if (e is ShieldTimesEffect shieldTimes) Put(r, 23, shieldTimes.value);
        else if (e is ExhaustRandomHandEffect exhaustRandom) Put(r, 23, exhaustRandom.value);
        else if (e is ExhaustAllHandDamageEffect exhaustAll) Put(r, 23, exhaustAll.value);
        // 重振精神：每张格挡走 W(23)、排除类型走 Y(25)
        else if (e is ExhaustHandShieldEffect exhaustShield)
        {
            Put(r, 23, exhaustShield.value);
            Put(r, 25, (int)exhaustShield.layer);
        }
        // 重刃：基础伤害 W(23)、力量倍率 Y(25)
        else if (e is DamageStrengthMultEffect strengthMult)
        {
            Put(r, 23, strengthMult.value);
            Put(r, 25, strengthMult.layer);
        }
        // 完美打击：基础伤害 W(23)、每张加成 Y(25)、牌名片段 X(24)
        else if (e is DamagePerKeyEffect perKey)
        {
            Put(r, 23, perKey.value);
            Put(r, 24, perKey.key);
            Put(r, 25, perKey.layer);
        }
        else if (e is DrainDamageEffect drain) Put(r, 23, drain.value);
        // 暴走：基础伤害 W(23)、每次递增 Y(25)
        else if (e is RampageEffect rampage)
        {
            Put(r, 23, rampage.value);
            Put(r, 25, rampage.layer);
        }
        // 挪牌：源堆 W(23)、目标堆 Y(25)
        else if (e is MoveTopCardEffect moveTop)
        {
            Put(r, 23, (int)moveTop.value);
            Put(r, 25, (int)moveTop.layer);
        }
        // CopyHandCardEffect / RandomAttackCardEffect 无自有字段

        var anim = e?.effectAnimData;
        if (anim != null)
        {
            Put(r, 27, anim.isAoe);
            Put(r, 28, anim.sprite != null ? anim.sprite.name : null);
            Put(r, 29, anim.duration);
        }
    }

    /// <summary>skill.rangeRule 的单个条件（Q/R 列）；多个条件各占一行。</summary>
    static void PutRangeCondition(Dictionary<int, object> r, RangeCondition cond)
    {
        if (cond == null) return;
        Put(r, 17, cond.GetType().Name);
        if (cond is RangeRandom rr) Put(r, 18, rr.count);
    }

    /// <summary>choiceRule 的单个条件（K/L 列）；多个条件各占一行。</summary>
    static void PutChoiceCondition(Dictionary<int, object> r, RangeCondition cond)
    {
        if (cond == null) return;
        Put(r, 11, cond.GetType().Name);
        if (cond is RangeRandom cr) Put(r, 12, cr.count);
    }

    static void Put(Dictionary<int, object> r, int col, object v)
    {
        if (v != null) r[col] = v;
    }
}
