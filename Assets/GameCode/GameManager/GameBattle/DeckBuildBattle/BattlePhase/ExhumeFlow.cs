using DG.Tweening;

/// <summary>
/// 发掘的表现：为取回的牌新建手牌 UI 并重排。
/// 数据已经由 ExhumeEffect.Apply 移进手牌，这里只补 UI（对应 BattleHandContainer.LoadData 的开局流程）。
/// </summary>
public class ExhumeFlow : BattleFlow
{
    public BattleCard card;

    public ExhumeFlow Start(BattleCard card)
    {
        this.card = card;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        if (card == null) return;

        var handContainer = DeckBuildBattle.GetUI().handContainer;
        var cardUI = handContainer.InstantiateObj(card);
        cardUI.CanvasGroup().SetActive(true);
        card.modified.interactiveMode.SetIdle(cardUI, true, true);

        handContainer.CalculateArg();
        handContainer.Arrange();
    }
}
