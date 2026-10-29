using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class HandCardArrangeFlow : BattleFlow
{
    public List<BattleCard> cards = new();
    public List<MotionData> data;
    public List<BattleCardUIMotion> cardUIMotions = new();
    public HandCardArrangeFlow Start(List<BattleCard> cards, List<MotionData> arrangeList)
    {
        this.cards.AddRange(cards);
        this.data = arrangeList;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            var ui = DeckBuildBattle.GetUI().handContainer.GetCardUI(card);
            var cardMotion = new BattleCardUIMotion().Start(ui, data[i], 0.2f);
            cardUIMotions.Add(cardMotion);
            flow.Join(cardMotion.flow);
        }
    }
}