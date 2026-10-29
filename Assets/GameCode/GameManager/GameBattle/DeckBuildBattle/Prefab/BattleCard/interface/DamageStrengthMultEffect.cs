using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 重刃：造成 value 点伤害，力量按 layer 倍计算（默认 3 倍）。
/// 不吃 ActiveModify_Damage 的力量加成 —— 这里是自己算，所以倍率必须一次算完，不能叠加系统加成。
/// value 走 W 列（基础伤害）、layer 走 Y 列（力量倍率）。
/// </summary>
public class DamageStrengthMultEffect : SkillEffect
{
    /// <summary>基础伤害（W 列）。</summary>
    [MinValue(1)] public int value;
    /// <summary>力量倍率（Y 列），默认 3。</summary>
    [MinValue(1)] public int layer = 3;

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"重刃 {GetType().Name} value 小于等于 0");
        if (layer <= 0)
            Debug.LogError($"重刃 {GetType().Name} layer 小于等于 0");
    }
    public override string Desc() => $"造成{value}点伤害，力量按{layer}倍计算";
    public override string IntentionDesc() => $"{value}";
    public override SkillEffect Create() => new DamageStrengthMultEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value, layer = layer };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var strength = 0;
        foreach (var modifier in caster.data.modifiers)
        {
            if (modifier.data.key != LimitBreakEffect.STRENGTH_KEY) continue;
            strength += modifier.data.layer;
        }

        var total = value + strength * layer;
        var overflow = target.data.shield - total;
        if (overflow >= 0)
        {
            target.data.shield = overflow;
            return new SkillEffectResult(damage: 0, heal: 0, shieldLost: total, string.Empty);
        }

        var shieldAbsorb = target.data.shield;
        // 2026-10-22 飘字/吸血用「格挡后的真实伤害」realDamage，不再以目标剩余血量为上限
        var realDamage = -overflow;
        var hpAfter = Mathf.Max(0, target.data.hp - realDamage);
        target.data.shield = 0;
        target.data.hp = hpAfter;
        return new SkillEffectResult(damage: realDamage, heal: 0, shieldLost: shieldAbsorb, string.Empty);
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new DamageFlow().Start(new Damage { effectAnimData = effectAnimData }, caster, target, result);
}
