using Sirenix.OdinInspector;
using UnityEngine;

public class DrawCardEffect : SkillEffect
{
    [MinValue(0)] public int value;
    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"抽牌 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"抽{value}张牌";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new DrawCardEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle)
    {
        return new NullGameModify();
    }
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle)
    {
        return new NullGameModify();
    }
    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        return default;
    }
    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        return new DrawCardFlow().Start(value, isSkillEffect: true, needSettle: true);
    }
}
