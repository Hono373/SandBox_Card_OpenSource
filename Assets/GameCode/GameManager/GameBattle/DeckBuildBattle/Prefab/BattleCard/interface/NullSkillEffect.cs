/// <summary>空技能效果：不产生任何数值/状态变更，也不播放表现。</summary>
public class NullSkillEffect : SkillEffect
{
    public override void ValidateData() { }
    public override string Desc() => "";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new NullSkillEffect() { hasSource = hasSource, effectAnimData = effectAnimData };
    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();
    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify) => default;
    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result) => new NullFlow().Start();
}
