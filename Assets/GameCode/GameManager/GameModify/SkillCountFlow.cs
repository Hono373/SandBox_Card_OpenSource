using DG.Tweening;
using System.Collections.Generic;

public class SkillCountFlow : BattleFlow
{
    public Skill skill;
    public List<BattleUnit> targets;
    public BattleUnit caster;
    public List<BattleFlow> skillFlows = new();
    public SkillCountFlow Start(List<BattleUnit> targets, Skill skill, BattleUnit caster)
    {
        this.targets = targets;
        this.skill = skill;
        this.caster = caster;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        foreach (var target in targets)
        {
            if (target.current.isDead) continue;
            var effect = skill.effect.Create();
            var passiveModify = effect.PassiveModify(caster, target, needSettle: true);
            var result = effect.Apply(caster, target, passiveModify);
            // Apply 已消费完 context，再结算 modifier 自身状态
            passiveModify.Settle();
            var skillFlow = effect.Start(caster, target, result);
            flow.Join(skillFlow.flow);
            skillFlows.Add(skillFlow);
        }
    }
}
