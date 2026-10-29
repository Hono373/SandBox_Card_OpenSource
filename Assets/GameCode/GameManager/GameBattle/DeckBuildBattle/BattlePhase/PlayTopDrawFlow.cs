using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 破灭的表现：按顺序结算抽牌堆顶那张牌的技能（目标用 AIChoice 自动选），最后把它移入消耗堆并播消耗事件。
/// </summary>
public class PlayTopDrawFlow : BattleFlow
{
    public PlayTopDrawEffect effect;
    public BattleUnit caster;
    public BattleCard picked;
    public List<Skill> skills = new();
    public ExhaustFlow exhaustFlow = new();

    public PlayTopDrawFlow Start(PlayTopDrawEffect effect, BattleUnit caster, BattleCard picked, List<Skill> skills)
    {
        this.effect = effect;
        this.caster = caster;
        this.picked = picked;
        this.skills.AddRange(skills);
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        if (picked == null) return;

        foreach (var skill in skills)
        {
            var choice = skill.rangeRule.AIChoice(caster);
            flow.Append(new SkillApplyFlow().Start(skill, caster, choice, 0, needSettle: true).flow);
        }

        // 那张牌不进手牌，直接进消耗堆（ExhaustFlow 负责移堆 + 动画 + 事件；
        // 它的 GetCardUI 只对手牌有效，所以这里手动处理）
        DeckBuildBattle.Active.deck.GetPile(PileEnum.Removed).Add(picked);
        new AfterExhaust().Start(caster, picked, true).AppendTo(flow);

        // 2026-10-23 抽牌堆顶被取走，抽牌堆数字要跟着变。
        flow.onComplete += () => DeckBuildBattle.GetUI().uiContainer.RefreshPileCount();
    }
}
