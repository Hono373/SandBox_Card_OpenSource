using System;
using DG.Tweening;

[Serializable]
public class BattleCardModify_InteractiveMode : GameModify
{
    public BattleCardModified current;
    public BattleCard card;
    public SortedWeight<BattleCardInteractive> weight = new();
    public BattleCardInteractive interactiveModeResult;
    public BattleCardModify_InteractiveMode Start(BattleCardModified current, bool needSettle)
    {
        this.current = current;
        card = current.Card;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        interactiveModeResult = weight.MaxOrDefault(() => current.Info.interactive.Create(card));
    }
}
