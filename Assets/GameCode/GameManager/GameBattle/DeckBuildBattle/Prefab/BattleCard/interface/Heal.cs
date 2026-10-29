using Sirenix.OdinInspector;
using UnityEngine;

public class Heal : SkillEffect
{
    [MinValue(0)] public int value;
    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        return new HealFlow().Start(this, caster, target, result);
    }
    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"治疗 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"恢复{value}点生命";
    public override SkillEffect Create() => new Heal() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle)
    {
        return new ActiveModify_Heal().Start(caster, this, needSettle);
    }
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle)
    {
        return new PassiveModify_Heal().Start(caster, target, this, needSettle);
    }
    public override string IntentionDesc() => $"{value}";
    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var hpAfter = Mathf.Min(target.data.maxHP, target.data.hp + value);
        target.data.hp = hpAfter;
        // 2026-10-22 飘字显示完整的治疗量 value，不按「还差多少血到满」截断
        return new SkillEffectResult(damage: 0, heal: value, shieldLost: 0, string.Empty);
    }
}
