using System;
using System.Collections.Generic;
using DG.Tweening;

public class DrawCardFlow : BattleFlow
{
    public bool needSettle;
    public bool isSkillEffect;
    public int drawNumb;
    public DrawCard drawCard = new();
    public List<BattleCard> handListBeforeDraw = new();
    public HandCardArrangeFlow handArrangeFlow = new();
    public HandCardCombineFlow handCombineFlow = new();

    public DrawCardFlow Start(int count, bool isSkillEffect, bool needSettle)
    {
        this.needSettle = needSettle;
        this.isSkillEffect = isSkillEffect;
        drawNumb = count;
        handListBeforeDraw.AddRange(DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list);
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        drawCard.Start(drawNumb, isSkillEffect, needSettle);
        var drawList = DeckBuildBattle.Active.deck.Draw(drawCard.drawNumbResult);
        DeckBuildBattle.Active.deck.AddTo(PileEnum.Hand, drawList);

        var argData = BattleCardUI.Anim.handArrangeCalculation.Result(handListBeforeDraw.Count + drawList.Count);
        var combineData = new List<MotionData>();
        for (var i = handListBeforeDraw.Count; i < handListBeforeDraw.Count + drawList.Count; i++)
        {
            combineData.Add(argData[i]);
        }

        handArrangeFlow.Start(handListBeforeDraw, argData);
        flow.Join(handArrangeFlow.flow);

        handCombineFlow.Start(drawList, combineData);
        flow.Append(handCombineFlow.flow);

        // 2026-10-22 抽到状态/诅咒牌事件（进化、火焰吐息订阅）。无色牌 = color None = 状态/诅咒。
        foreach (var card in drawList)
        {
            if (CardInfo.Get(card.data.key).graphic.color != ColorEnum.None) continue;
            new AfterDrawStatus().Start(card.modified.owner, card, true).AppendTo(flow);
        }

        DeckBuildBattle.GetUI().handContainer.CalculateArg();

        // 2026-10-23 抽牌会让抽牌堆减少；若抽到中途触发 Refill，弃牌堆还会被清空洗回。
        // RefreshDrawPilecheckUIFlow 的递进数字是用开始前的缓存算的，Refill 后会算少，
        // 所以整套抽牌动画结束后按 deck 真实数量兜一次。
        flow.onComplete += () => DeckBuildBattle.GetUI().uiContainer.RefreshPileCount();
    }
}
