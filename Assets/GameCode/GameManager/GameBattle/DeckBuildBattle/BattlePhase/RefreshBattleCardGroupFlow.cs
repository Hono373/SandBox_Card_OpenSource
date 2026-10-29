using System.Collections.Generic;

public class RefreshBattleCardGroupFlow : BattleFlow
{
    public bool needSettle;
    public List<BattleCardModify> cardModifys = new();
    public List<BattleCardUIShot> cardUIShots = new();

    public RefreshBattleCardGroupFlow Start(bool needSettle)
    {
        this.needSettle = needSettle;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        foreach (var card in DeckBuildBattle.Active.deck.GetPile(PileEnum.TotalPile).list)
        {
            cardModifys.Add(new BattleCardModify().Start(card, needSettle));
        }

        foreach (var card in DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list)
        {
            var ui = DeckBuildBattle.GetUI().handContainer.GetCardUI(card);
            var shot = new BattleCardUIShot(ui.manager.modified);
            cardUIShots.Add(shot);
            flow.onComplete += () => ui.Refresh(shot);
        }
    }
}
