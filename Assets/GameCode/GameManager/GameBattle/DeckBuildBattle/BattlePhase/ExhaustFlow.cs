using System;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 消耗流程：把一批牌移出手牌 → 移入消耗堆（PileEnum.Removed）→ 逐张播消失动画 → 广播 AfterExhaust。
/// **「哪些牌要被消耗」由调用方传入**（显式列表 或 判定函数），流程本身不做筛选。
///
/// 封装理由：这套「移堆 + 销毁动画 + 事件」原来在 ExhaustHandFlow / ExhaustHandShieldFlow /
/// ExhaustAllHandDamageFlow 三处各有一份，改一个漏两个。现在只有这一份来源。
///
/// 用法：
///   断魂斩   new ExhaustFlow().Start(PileEnum.Hand, c => CardInfo.Get(c.data.key).graphic.type != TypeEnum.Attack)
///   坚毅     new ExhaustFlow().Start(pickedRandomCards)
///   恶魔之焰 new ExhaustFlow().Start(PileEnum.Hand, _ => true)   // 全烧
/// </summary>
public class ExhaustFlow : BattleFlow
{
    /// <summary>本次要消耗的牌（按传入顺序）。</summary>
    public List<BattleCard> cards = new();
    public List<CardDestroyFlow> cardDestroyFlows = new();
    /// <summary>消耗完是否重排手牌（手牌全清时不用重排）。</summary>
    public bool arrangeHand = true;

    /// <summary>显式指定要消耗的牌。</summary>
    public ExhaustFlow Start(List<BattleCard> cards)
    {
        this.cards.AddRange(cards);
        Init();
        return this;
    }
    /// <summary>按判定从指定牌堆里挑（目前只支持手牌：销毁动画取自手牌容器）。</summary>
    public ExhaustFlow Start(PileEnum fromPile, Func<BattleCard, bool> predicate)
    {
        foreach (var card in DeckBuildBattle.Active.deck.GetPile(fromPile).list)
        {
            if (!predicate(card)) continue;
            cards.Add(card);
        }
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var handPile = DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand);
        var exhaustPile = DeckBuildBattle.Active.deck.GetPile(PileEnum.Removed);
        var ui = DeckBuildBattle.GetUI();

        foreach (var card in cards)
        {
            var cardUI = ui.handContainer.GetCardUI(card);
            cardUI.stateEnum = CardStateEnum.Destroy;

            handPile.Remove(card);
            exhaustPile.Add(card);

            var cardDestroyFlow = new CardDestroyFlow().Start(cardUI);
            cardDestroyFlows.Add(cardDestroyFlow);
            flow.Join(cardDestroyFlow.flow);

            // 消耗事件：黑暗之拥 / 无惧疼痛等 buff 订阅
            new AfterExhaust().Start(card.modified.owner, card, true).AppendTo(flow);
        }

        if (!arrangeHand) return;
        ui.handContainer.CalculateArg();
        ui.handContainer.Arrange();
    }
}
