using System;
using DG.Tweening;

[Serializable]
public class BattleCardModify_Frequency : GameModify
{
    public int frequencyModifi;
    public BattleCardModified cardModified;
    public int frequencyResult;
    public BattleCardModify_Frequency Start(BattleCardModified current,bool needSettle)
    {
        this.cardModified = current;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        frequencyResult = (1 + frequencyModifi).GreaterThanOrEqual(0);
    }
}
