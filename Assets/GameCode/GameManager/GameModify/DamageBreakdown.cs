// 2026-10-23 伤害明细记录：把一次伤害的「卡面值 → 力量 → 易伤 → 实际值」记成一条。
//
// 为什么按「一次命中」组织而不是按「一次修正」：一开始我按阶段各记一条，浮层里就变成
// 「[施法者] 火大地精 4 → 无修正 → 4」「[目标] 铁甲战士 4 → 无修正 → 4」两行 —— 同一件事
// 说两遍，而且读起来像「4 变成 4」。人要看的是「谁打了谁、掉了多少、中间改了几次」，
// 所以施法者阶段只做暂存，等目标阶段落定后合成一条。
//
// 两个必须记住的约束：
//   1. 同一条链会跑两遍 —— BattleUnitSkill 用 needSettle:false 跑预演（卡面数字），
//      真结算才 needSettle:true。调用方必须按 needSettle 过滤，否则每条伤害记两次。
//   2. damage.value 是原地改写，所以「卡面值」必须在 ModifyAndCreateResult 之前取。
using System.Collections.Generic;

public struct DamageHit
{
    /// <summary>施法者名。空串表示这一击没抓到 Active 阶段（比如直接伤害），渲染时省略箭头。</summary>
    public string caster;
    public string target;
    /// <summary>卡面值，力量之前</summary>
    public int baseValue;
    /// <summary>施法者侧的定点修正（目前只有力量，按层数）</summary>
    public int casterFlat;
    /// <summary>目标侧的定点修正（StrengthEffect 只吃施法者侧，所以当前恒为 0，留着是为了以后加护甲/减伤）</summary>
    public int targetFlat;
    /// <summary>倍率修正，50 表示 ×1.5（目前只有易伤写它）</summary>
    public int percent;
    /// <summary>forceResult 直接改写，此时中间几步算不出终值</summary>
    public bool forced;
    public int finalValue;
    /// <summary>连续相同命中的次数，AOE 打同一只怪时合并成「×2」而不是刷两行</summary>
    public int repeat;

    /// <summary>没有发生任何修正时，浮层只显示一行结果，不摆过程</summary>
    public bool Modified => casterFlat != 0 || targetFlat != 0 || percent != 0 || forced;
}

public static class DamageBreakdown
{
    /// <summary>只留最近这么多条，浮层放得下就够</summary>
    public const int CAP = 8;

    static readonly List<DamageHit> hits = new(CAP);

    // 施法者侧暂存：一次出手可能打多个目标（AOE），所以 Complete 之后不清空，
    // 等下一次 Begin 覆盖。用施法者名做配对键，不匹配就当这一击没有施法者阶段。
    static bool hasPending;
    static string pendingCaster;
    static int pendingBase;
    static int pendingFlat;

    /// <summary>关掉后不再记录，避免把调试浮层误带进正式包</summary>
    public static bool Enabled = true;

    public static IReadOnlyList<DamageHit> Hits => hits;

    /// <summary>施法者侧结算完调用（ActiveModify_Damage）。</summary>
    public static void BeginCaster(string caster, int baseValue, int flat)
    {
        if (!Enabled) return;
        hasPending = true;
        pendingCaster = caster;
        pendingBase = baseValue;
        pendingFlat = flat;
    }

    /// <summary>目标侧结算完调用（PassiveModify_Damage）。entering 是进目标侧时的值，含力量。</summary>
    public static void CompleteTarget(string caster, string target, int entering,
                                      int targetFlat, int percent, bool forced, int finalValue)
    {
        if (!Enabled) return;

        DamageHit hit;
        // 施法者名字直接取参数 —— 暂存只用来补「卡面值 / 力量」这两个 Active 阶段才有的量。
        // 早先版本只在暂存命中时才填 caster，玩家出牌的伤害（Active 阶段走预演不计）就变成
        // 只有目标没有施法者的一行，读起来没头没尾。
        hit.caster = caster;
        if (hasPending && pendingCaster == caster)
        {
            hit.baseValue = pendingBase;
            hit.casterFlat = pendingFlat;
        }
        else
        {
            hit.baseValue = entering;
            hit.casterFlat = 0;
        }

        hit.target = target;
        hit.targetFlat = targetFlat;
        hit.percent = percent;
        hit.forced = forced;
        hit.finalValue = finalValue;
        hit.repeat = 1;

        // 连续一模一样的命中（AOE 打同一只怪）合并计数，避免刷屏
        if (hits.Count > 0)
        {
            int last = hits.Count - 1;
            DamageHit prev = hits[last];
            if (prev.caster == hit.caster && prev.target == hit.target
                && prev.baseValue == hit.baseValue && prev.casterFlat == hit.casterFlat
                && prev.targetFlat == hit.targetFlat && prev.percent == hit.percent
                && prev.finalValue == hit.finalValue)
            {
                prev.repeat += 1;
                hits[last] = prev;
                return;
            }
        }

        hits.Add(hit);
        if (hits.Count > CAP)
            hits.RemoveAt(0);
    }

    /// <summary>换场 / 新战斗时清空暂存，免得上一场的力量配到这一击上</summary>
    public static void Clear()
    {
        hits.Clear();
        hasPending = false;
    }
}
