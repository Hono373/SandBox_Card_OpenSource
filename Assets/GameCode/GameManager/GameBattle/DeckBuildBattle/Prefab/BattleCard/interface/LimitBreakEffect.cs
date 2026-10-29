using UnityEngine;

/// <summary>
/// 力量翻倍（突破极限）。
/// 只对身上已存在的**正值**力量生效：无力量或负力量时什么都不做（返回 blocked），
/// 避免出现「负力量翻倍变得更负」的反直觉结果。
/// </summary>
public class LimitBreakEffect : SkillEffect
{
    public const string STRENGTH_KEY = "Strength";

    public override void ValidateData() { }

    public override string Desc() => "将力量翻倍";

    public override string IntentionDesc() => "";

    public override SkillEffect Create() => new LimitBreakEffect() { hasSource = hasSource, effectAnimData = effectAnimData };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();

    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        if (target == null || target.data == null)
            return new SkillEffectResult(0, 0, 0, string.Empty) { blocked = true };

        var exist = target.data.modifiers.Find(m => m != null && m.data.key == STRENGTH_KEY);
        if (exist == null || exist.data.layer <= 0)
        {
            Debug.Log($"[LimitBreak] {target.data.id} 没有正值力量，力量翻倍不生效");
            return new SkillEffectResult(0, 0, 0, string.Empty) { blocked = true };
        }

        exist.data.layer *= 2;
        return new SkillEffectResult(0, 0, 0, exist.data.id);
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        // 2026-10-22 补：原来直接返回 NullFlow，力量翻倍后 buff 图标层数不变（显示的还是旧值）。
        // result.modifierID 指向被翻倍的那个力量实例，交给 buff 栏 AddOrRefresh 重读层数。
        if (result.blocked) return new NullFlow().Start();

        var modifier = GameModifyCenter.modifiers[result.modifierID];
        DeckBuildBattle.GetUI().unitContainer.GetUnitUI(target).modifierBarUI.AddOrRefresh(modifier);
        return new NullFlow().Start();
    }
}
