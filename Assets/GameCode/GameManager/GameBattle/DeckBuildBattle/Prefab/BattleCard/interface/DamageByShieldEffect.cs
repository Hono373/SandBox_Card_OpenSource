using UnityEngine;

/// <summary>
/// 全身撞击：造成等同于自己**当前格挡值**的伤害。
/// 无字段 —— 伤害来源是 caster.data.shield，读的是结算那一刻的值，所以放在 Apply 里取。
/// 表现复用 DamageFlow（只需要 effectAnimData，不看 value）。
/// </summary>
public class DamageByShieldEffect : SkillEffect
{
    public override void ValidateData() { }
    public override string Desc() => "造成等同于当前格挡的伤害";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new DamageByShieldEffect() { hasSource = hasSource, effectAnimData = effectAnimData };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var value = caster.data.shield;
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
        return new SkillEffectResult(damage: realDamage, heal: 0, shieldLost: shieldAbsorb, string.Empty);
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new DamageFlow().Start(new Damage { effectAnimData = effectAnimData }, caster, target, result);
}
