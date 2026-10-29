using System;
using System.Collections.Generic;
using DG.Tweening;
public class RefreshUnitIntentionGroupFlow : BattleFlow
{
    public bool needSettle;
    public ICollection<BattleUnit> units;
    public List<RefreshUnitIntentionFlow> refreshUnitIntentions = new();
    public RefreshUnitIntentionGroupFlow Start(ICollection<BattleUnit> units, bool needSettle)
    {
        this.needSettle = needSettle;
        this.units = units;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        foreach (var unit in units)
        {
            // 重抽数据在这里显式做：RefreshUnitIntentionFlow 本身只负责刷 UI。
            RefreshUnitIntentionFlow.RefreshData(unit);
            var refreshUnitIntention = new RefreshUnitIntentionFlow().Start(unit);
            refreshUnitIntentions.Add(refreshUnitIntention);
            flow.Join(refreshUnitIntention.flow);
        }
    }
}
