using System;

/// <summary>阶段技能：在指定战斗阶段触发配置的技能</summary>
[Serializable]
public class PhaseSkill : ModifierEffect<OnPhaseStartModify>
{
    public PhaseEnum phaseEnum = PhaseEnum.PlayerPhaseEnd;
    public Skill skill;
    public override ModifierEffect Create() => new PhaseSkill() { phaseEnum = phaseEnum, skill = skill.Create() };
    public override void Modify(Modifier modifier, OnPhaseStartModify context)
    {
        if (!Match(modifier, context.phase))
            return;
        context.skillApplyFlows.Add(new SkillApplyFlow().Start(skill, modifier.GetOwner(), null, 0, true));
    }
    bool Match(Modifier modifier, object phase) => phaseEnum switch
    {
        PhaseEnum.Any => false,
        PhaseEnum.BattleStart => phase is BattlePhaseStart,
        PhaseEnum.PlayerPhaseStart => phase is PlayerPhaseStart,
        PhaseEnum.PlayerPhaseEnd => phase is PlayerPhaseEnd,
        PhaseEnum.EnemyPhaseStart => phase is EnemyPhaseStart,
        PhaseEnum.EnemyPhaseEnd => phase is EnemyPhaseEnd,
        PhaseEnum.Death => phase is DeathSkillPhase death && death.unit == modifier.GetOwner(), 
    };
}
