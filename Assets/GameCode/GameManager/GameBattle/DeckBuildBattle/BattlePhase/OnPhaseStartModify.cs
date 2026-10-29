using System.Collections.Generic;
using DG.Tweening;

public class OnPhaseStartModify : GameModify
{
    public object phase;
    public List<SkillApplyFlow> skillApplyFlows = new();
    public OnPhaseStartModify Start(object phase, bool needSettle)
    {
        this.phase = phase;
        ModifyAndCreateResult(needSettle);
        return this;
    }
}
