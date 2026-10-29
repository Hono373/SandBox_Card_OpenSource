using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 破灭：打出抽牌堆顶的一张牌，然后把它消耗掉。
/// 目标由 `RangeRule.AIChoice(caster)` 自动决定（跟敌人出招同一套逻辑：GetTargetList + conditions + 随机），
/// 所以抽到「痛击」这类需要选目标的卡也不会崩。
/// 只结算该卡 ActiveUse 时机的技能；费用不收（不打出手牌，也不走 BattleCardUsePhaseFlow）。
/// </summary>
public class PlayTopDrawEffect : SkillEffect
{
    /// <summary>本次打出的牌（Apply 写、Start 读）。</summary>
    [System.NonSerialized] public BattleCard picked;
    /// <summary>要结算的技能副本（Create 过的，不污染表数据）。</summary>
    [System.NonSerialized] public List<Skill> skills = new();

    public override void ValidateData() { }
    public override string Desc() => "打出抽牌堆顶的一张牌，并将其消耗";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new PlayTopDrawEffect() { hasSource = hasSource, effectAnimData = effectAnimData };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var drawPile = DeckBuildBattle.Active.deck.GetPile(PileEnum.Draw);
        if (drawPile.Count() == 0) return default;

        picked = drawPile.Index(0);
        drawPile.Remove(picked);

        foreach (var timing in picked.modified.timings)
        {
            if (timing.timingEnum != TimingEnum.ActiveUse) continue;
            foreach (var skill in timing.skills)
                skills.Add(skill.Create());
            break;
        }
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new PlayTopDrawFlow().Start(this, caster, picked, skills);
}
