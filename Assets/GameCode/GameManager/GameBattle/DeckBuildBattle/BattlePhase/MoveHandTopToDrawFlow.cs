using DG.Tweening;

/// <summary>战吼的表现：被移走的那张手牌播消失动画，然后重排剩下的手牌。</summary>
public class MoveHandTopToDrawFlow : BattleFlow
{
    public BattleCard card;
    public CardDestroyFlow cardDestroyFlow = new();

    public MoveHandTopToDrawFlow Start(BattleCard card)
    {
        this.card = card;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        if (card == null) return;

        var handContainer = DeckBuildBattle.GetUI().handContainer;
        var cardUI = handContainer.GetCardUI(card);
        cardUI.stateEnum = CardStateEnum.Destroy;
        flow.Join(cardDestroyFlow.Start(cardUI).flow);

        handContainer.CalculateArg();
        handContainer.Arrange();
    }
}
