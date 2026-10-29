using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 死亡收割：造成 value 点伤害，未被格挡的那部分**回复自己生命**。
/// AOE 时 SkillApplyFlow 会对每个目标各调一次 Apply，所以每段未格挡伤害都会回血 —— 正好是想要的效果。
/// value 走 W 列。
/// </summary>
public class DrainDamageEffect : SkillEffect
{
    /// <summary>伤害值（W 列）。</summary>
    [MinValue(1)] public int value;

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"吸血伤害 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"造成{value}点伤害，未被格挡的伤害回复生命";
    public override string IntentionDesc() => $"{value}";
    public override SkillEffect Create() => new DrainDamageEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var overflow = target.data.shield - value;
        if (overflow >= 0)
        {
            target.data.shield = overflow;
            return new SkillEffectResult(damage: 0, heal: 0, shieldLost: value, string.Empty);
        }

        var shieldAbsorb = target.data.shield;
        // 2026-10-22 飘字/吸血用「格挡后的真实伤害」realDamage，不再以目标剩余血量为上限
        var realDamage = -overflow;
        var hpAfter = Mathf.Max(0, target.data.hp - realDamage);
        target.data.shield = 0;
        target.data.hp = hpAfter;

        // 未格挡的伤害回复自身（实际回血受 maxHP 限制，飘字仍显示完整的未格挡伤害量）
        var casterHpAfter = Mathf.Min(caster.data.maxHP, caster.data.hp + realDamage);
        caster.data.hp = casterHpAfter;

        return new SkillEffectResult(damage: realDamage, heal: realDamage, shieldLost: shieldAbsorb, string.Empty);
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new DrainDamageFlow().Start(this, caster, target, result);
}
