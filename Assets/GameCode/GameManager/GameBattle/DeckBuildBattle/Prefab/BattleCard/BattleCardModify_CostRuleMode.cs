using System;
using DG.Tweening;

[Serializable]
public class BattleCardModify_CostRuleMode : GameModify
{
    public bool needSettle;
    public BattleCardModified modified;
    public SortedWeight<BattleCardCostRule> weight = new();

    public BattleCardCostRule costRuleResult;
    public BattleCardModify_CostRuleMode Start(BattleCardModified modified, bool needSettle)
    {
        this.modified = modified;
        this.needSettle = needSettle;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        costRuleResult = weight.MaxOrDefault(modified.Info.cost.Create);
    }
}
