using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleCardUseFlow : BattleFlow
{
    public readonly bool needSettle = true;
    public int costUsed;
    public int costResult;
    public int maxCostResult;
    public BattleCard card;
    public BattleUnit caster;
    public BattleUnit choice;
    public RefreshBattleCardGroupFlow refreshBattleCardGroupFlow = new();
    public BattleUnitActionMove actionMove = new();
    public List<SkillApplyFlow> skillPhases = new();
    public BattleCardUseFlow Start(BattleCard card, BattleUnit choice)
    {
        this.card = card;
        this.caster = card.modified.owner;
        this.choice = choice;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        costUsed = card.modified.costRule.GetResult();
        DeckBuildBattle.Active.data.energy -= costUsed;

        costResult = DeckBuildBattle.Active.data.energy;
        maxCostResult = DeckBuildBattle.Active.data.maxEnergy;

        // 2026-10-20 仅攻击牌前冲，Skill / Ability 牌不位移；一次出牌只冲一次，放在技能结算之前。
        // 2026-10-22 位移改用 Join：前冲回位（0.12 + 0.18）不再单独占满一整段，而是与技能结算并行。
        // 注意 Sequence.Join 是「与最后一个 tween 同起点」，所以技能结算也必须用 Join 才真并行；
        // 若技能仍 Append，它会排在 max(位移) 之后，等于顺序播放、白改。
        var activeUseFlow = GameAnim.Flow();
        foreach (var timing in card.modified.timings)
        {
            if (timing.timingEnum == TimingEnum.ActiveUse)
            {
                for (var i = 0; i < card.modified.frequency; i++)
                {
                    var ferquencyFlow = GameAnim.Flow();
                    foreach (var skill in timing.skills)
                    {
                        var skillPhaseFlow = new SkillApplyFlow().Start(skill, caster, choice, costUsed, needSettle);
                        skillPhases.Add(skillPhaseFlow);
                        ferquencyFlow.Join(skillPhaseFlow.flow);
                    }
                    // 多段频率之间保持串行，整体再与位移并行
                    activeUseFlow.Append(ferquencyFlow);
                }
            }
        }

        if (card.modified.graphic.type == TypeEnum.Attack)
            flow.Join(actionMove.Start(caster).flow);
        flow.Join(activeUseFlow);

        foreach (var timing in card.modified.timings)
        {
            if (timing.timingEnum == TimingEnum.ActiveUse)
            {
                timing.AfterUse(card);
            }
        }
        flow.Append(refreshBattleCardGroupFlow.Start(needSettle: false).flow);

        // 2026-10-23 打出的牌在 AfterUse 里当场就进了弃牌堆 / 消耗堆 / Destroy 堆，
        // 但数字要等整段动画演完再变，所以挂 onComplete 统一重读两个堆。
        flow.onComplete += () => DeckBuildBattle.GetUI().uiContainer.RefreshPileCount();
    }
}