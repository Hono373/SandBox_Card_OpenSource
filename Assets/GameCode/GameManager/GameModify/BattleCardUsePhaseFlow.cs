using DG.Tweening;
using System.Collections.Generic;

public class BattleCardUsePhaseFlow : RootBattleFlow
{
    public readonly bool needSettle = true;
    public BattleCard card;
    public BattleUnit choice;
    public BattleCardModify battleCardModify = new();
    public BattleCardUseFlow battleCardUseFlow = new();
    public CardDestroyFlow cardDestroyFlow = new();
    public BattleEndCheckFlow battleEndCheckFlow = new();
    public BattleCardUsePhaseFlow Start(BattleCard card, BattleUnit choice)
    {
        this.card = card;
        this.choice = choice;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var ui = DeckBuildBattle.GetUI().handContainer.GetCardUI(card);
        ui.stateEnum = CardStateEnum.Destroy;

        battleCardModify.Start(card, needSettle);

        DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).Remove(card);

        battleCardUseFlow.Start(card, choice);

        flow.Join(DeckBuildBattle.GetUI().uiContainer.RefreshCostUI(battleCardUseFlow.costResult, battleCardUseFlow.maxCostResult));
        flow.Join(cardDestroyFlow.Start(ui).flow);

        flow.Join(battleCardUseFlow.flow);

        DeckBuildBattle.GetUI().handContainer.CalculateArg();
        DeckBuildBattle.GetUI().handContainer.Arrange();

        flow.Append(battleEndCheckFlow.Start().flow);

        // 2026-10-22 打出攻击牌事件（狂怒订阅）
        if (card.modified.graphic.type == TypeEnum.Attack)
            new AfterPlayAttack().Start(card.modified.owner, card, true).AppendTo(flow);
    }
}
