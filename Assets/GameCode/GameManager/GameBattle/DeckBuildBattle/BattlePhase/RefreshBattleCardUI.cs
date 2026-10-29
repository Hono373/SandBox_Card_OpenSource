using System;
using DG.Tweening;

public class RefreshBattleCardUI : BattleFlow
{
    public BattleCard card;
    /// <summary>外观数据源，必传：常规刷新传 card.modified，指向目标等预演场景传临时副本。</summary>
    public BattleCardModified modified;
    public BattleCardUI ui;
    public BattleCardUIShot cardUIData;
    public RefreshBattleCardUI Start(BattleCard card, BattleCardModified modified)
    {
        if (card == null || modified == null)
        {
            GameDebug.Log($"{nameof(RefreshBattleCardUI)}.Start: card 或 modified 为 null，错误调用！");
            return this;
        }
        this.card = card;
        this.modified = modified;
        ui = DeckBuildBattle.GetUI().handContainer.GetCardUI(card);
        Init();
        flow.Play();
        return this;
    }
    protected override void FlowBuild()
    {
        cardUIData = new BattleCardUIShot(modified);
        flow.onComplete += () => ui.Refresh(cardUIData);
    }
}
