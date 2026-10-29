using System;

public abstract class SkillEffect
{
    public bool hasSource = true;
    public EffectAnimData effectAnimData = new();
    public abstract void ValidateData();
    public abstract string Desc();
    public abstract string IntentionDesc();
    public abstract SkillEffect Create();
    public abstract GameModify ActiveModify(BattleUnit caster, bool needSettle);
    public abstract GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle);
    public abstract BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result);
    public abstract SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify);
}
