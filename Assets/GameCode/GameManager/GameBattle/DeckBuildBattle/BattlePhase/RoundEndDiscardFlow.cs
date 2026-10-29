using System.Collections.Generic;
using DG.Tweening;

public class RoundEndDiscardFlow : BattleFlow
{
    public List<BattleCard> discards = new();
    public List<CardDestroyFlow> cardDestroyFlows = new();
    public RoundEndDiscardFlow Start()
    {
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var deck = DeckBuildBattle.Active.deck;
        var handPile = deck.GetPile(PileEnum.Hand);

        discards.AddRange(handPile.list);
        foreach (var card in discards)
        {
            // 2026-10-22 手牌上的「回合结束」时机（灼伤这类状态牌）：不可打出，但带 PlayerPhaseEnd 的 timing。
            // 能力牌走 CardPhaseTimingFlow（只遍历 powers），状态牌永远在 powers 之外，所以这里单独结算。
            foreach (var timing in card.modified.timings)
            {
                if (timing.timingEnum != TimingEnum.PlayerPhaseEnd) continue;
                foreach (var skill in timing.skills)
                {
                    var copy = skill.Create();
                    flow.Append(new SkillApplyFlow()
                        .Start(copy, card.modified.owner, copy.rangeRule.AIChoice(card.modified.owner), 0, needSettle: true).flow);
                }
            }

            var ui = DeckBuildBattle.GetUI().handContainer.GetCardUI(card);
            ui.stateEnum = CardStateEnum.Destroy;

            // 2026-10-22 虚无（Ethereal）：回合结束时仍在手牌 → 进消耗堆，不进弃牌堆
            // （消耗堆不参与 Refill，所以这张牌本场不会再被抽到；发掘之类可以从这里取回）
            var pile = CardInfo.Get(card.data.key).ethereal ? PileEnum.Removed : PileEnum.Discard;

            handPile.Remove(card);
            deck.GetPile(pile).Add(card);

            // 2026-10-22 虚无进消耗堆同样算「被消耗」，要广播事件
            if (pile == PileEnum.Removed)
                new AfterExhaust().Start(card.modified.owner, card, true).AppendTo(flow);

            var cardDestroyFlow = new CardDestroyFlow().Start(ui);
            cardDestroyFlows.Add(cardDestroyFlow);
            flow.Join(cardDestroyFlow.flow);
        }

        // 2026-10-23 手牌在这里整批进了弃牌堆（虚无的进消耗堆），弃牌堆数字要跟着变。
        flow.onComplete += () => DeckBuildBattle.GetUI().uiContainer.RefreshPileCount();
    }
}
