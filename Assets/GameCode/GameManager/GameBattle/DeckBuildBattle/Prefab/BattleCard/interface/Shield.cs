using Sirenix.OdinInspector;
using UnityEngine;

public class Shield : SkillEffect
{
    [MinValue(0)] public int value;

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"护盾 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"提供{value}点护盾";
    public override string IntentionDesc() => $"{value}";
    public override SkillEffect Create() => new Shield() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle)
    {
        return new ActiveModify_Shield().Start(caster, this, needSettle);
    }
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle)
    {
        return new PassiveModify_Shield().Start(caster, target, this, needSettle);
    }
    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        target.data.shield = target.data.shield + value;
        // 2026-10-22 获得格挡事件（势不可当订阅）
        new AfterGainShield().Start(target, value, true).PlayFlows();
        return default;
    }
    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        return new ShieldFlow().Start(this, caster, target);
    }
}
