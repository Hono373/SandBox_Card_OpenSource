using System.Collections.Generic;
using DG.Tweening;

public class BattleStartSkillFlow : BattleFlow
{
    public bool needSettle;
    public ICollection<BattleUnit> units;
    public List<SkillApplyFlow> skillApplyFlows = new();
    public BattleStartSkillFlow Start(ICollection<BattleUnit> units, bool needSettle)
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
            foreach (var skill in unit.Info.battleStartSkills)
            {
                var skillApplyFlow = new SkillApplyFlow().Start(skill.Create(), unit, null, 0, needSettle);
                skillApplyFlows.Add(skillApplyFlow);
                flow.Join(skillApplyFlow.flow);
            }
        }
    }
}
