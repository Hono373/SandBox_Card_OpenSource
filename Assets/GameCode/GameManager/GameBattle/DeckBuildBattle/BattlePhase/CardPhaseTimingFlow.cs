using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 按相位触发「常驻能力牌」自身的 timings。
///
/// 背景：卡牌的 timings 原本只在 BattleCardUseFlow（打牌那一刻）里执行，
/// 所以「回合结束时获得格挡」这类效果无法表达。
/// 能力牌打出后不再进弃牌堆，而是登记到 DeckBuildBattle.Active.powers（见
/// BattleTiming.AfterUse）；本流程在玩家/敌人回合开始、结束时遍历这些牌，
/// 挑出 timingEnum 匹配的 skill 逐条结算。
///
/// 触发时没有玩家点选动作，因此 choice 传 null；技能作用对象完全由其
/// rangeRule 决定（Self 用 active=true + baseRange=3）。
/// </summary>
public class CardPhaseTimingFlow : BattleFlow
{
    public TimingEnum timing;
    public bool needSettle;
    public List<SkillApplyFlow> skillApplyFlows = new();

    public CardPhaseTimingFlow Start(TimingEnum timing, bool needSettle)
    {
        this.timing = timing;
        this.needSettle = needSettle;
        Init();
        return this;
    }

    protected override void FlowBuild()
    {
        skillApplyFlows.Clear();

        var battle = DeckBuildBattle.Active;
        if (battle == null || battle.powers == null || battle.powers.Count == 0) return;

        foreach (var card in battle.powers)
        {
            var modified = card?.modified;
            if (modified == null || modified.timings == null) continue;

            foreach (var t in modified.timings)
            {
                if (t == null || t.timingEnum != timing || t.skills == null) continue;

                foreach (var skill in t.skills)
                {
                    if (skill == null) continue;
                    var skillFlow = new SkillApplyFlow()
                        .Start(skill, modified.owner, null, 0, needSettle);
                    skillApplyFlows.Add(skillFlow);
                    flow.Append(skillFlow.flow);
                }
            }
        }
    }
}
