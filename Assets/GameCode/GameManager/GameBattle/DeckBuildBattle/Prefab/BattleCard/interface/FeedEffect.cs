using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 狂宴：造成 value 点伤害；若这次伤害造成击杀，自己**永久**获得 layer 点最大生命。
/// 卡本身消耗（timing.pileEnum = 5）。
///
/// 为什么不复用 Damage：伤害结算完必须立刻知道"目标死没死"才能决定加不加最大生命，
/// 而 Damage 只返回一个伤害数字、没有这个出口。给 Damage 加字段要动表结构（effect 子字段被
/// 表头合并区锁在 U~AC 已排满），单独写一个类更便宜 —— 用户 2026-10-22 明确"专门写类，没时间抽象"。
///
/// 字段对应表列：value -> W(23)、layer -> Y(25)、effectAnimData -> AA~AC。零表结构改动。
/// </summary>
public class FeedEffect : SkillEffect
{
    /// <summary>伤害值（W 列）。</summary>
    [MinValue(1)] public int value;
    /// <summary>击杀时永久获得的极大生命值（Y 列 layer）。</summary>
    [MinValue(1)] public int layer = 3;

    /// <summary>本次结算是否造成击杀。Apply 写入、Start 读取 —— 两者拿到的是同一个 effect 实例
    /// （见 SkillApplyFlow 里 `var effect = skill.effect.Create();` 之后连续调 Apply / Start）。
    /// 不进表、不进存档。</summary>
    [System.NonSerialized] public bool killed;

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"狂宴 {GetType().Name} value 小于等于 0");
        if (layer <= 0)
            Debug.LogError($"狂宴 {GetType().Name} layer 小于等于 0");
    }
    public override string Desc() => $"造成{value}点伤害。若斩杀，则永久获得{layer}点最大生命";
    public override string IntentionDesc() => $"{value}";
    public override SkillEffect Create() => new FeedEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value, layer = layer };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        // 伤害结算沿用 Damage 的"护盾优先"规则：先扣盾，溢出部分扣血
        var overflow = target.data.shield - value;
        if (overflow >= 0)
        {
            target.data.shield = overflow;
            killed = false;
            return new SkillEffectResult(damage: 0, heal: 0, shieldLost: value, string.Empty);
        }

        var shieldAbsorb = target.data.shield;
        // 2026-10-22 飘字/吸血用「格挡后的真实伤害」realDamage，不再以目标剩余血量为上限
        var realDamage = -overflow;
        var hpAfter = Mathf.Max(0, target.data.hp - realDamage);
        target.data.shield = 0;
        target.data.hp = hpAfter;

        killed = target.data.hp <= 0;
        return new SkillEffectResult(damage: realDamage, heal: 0, shieldLost: shieldAbsorb, string.Empty);
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new FeedFlow().Start(this, caster, target, result);
}
