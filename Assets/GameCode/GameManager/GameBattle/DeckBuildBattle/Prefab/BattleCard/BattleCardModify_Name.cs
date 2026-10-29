using System;
using DG.Tweening;

[Serializable]
public class BattleCardModify_Name : GameModify
{
    public BattleCardModified current;
    public string cardName;
    public string cardNamePrefix = "";
    public string cardNameReplace = "";
    public string cardNameSuffix = "";
    public string cardNameResult;
    public BattleCardModify_Name Start(BattleCardModified current, bool needSettle)
    {
        this.current = current;
        cardName = current.Info.cardName;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        cardNameResult = string.Join(cardNamePrefix, cardNameReplace is "" ? cardName : cardNameReplace, cardNameSuffix);
    }
}
