using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class DeathSkillPhase : BattleFlow
{
    public BattleEndCheckFlow check;
    public BattleUnit unit;
    public OnPhaseStartModify onPhaseStartModify = new();
    public DeathSkillPhase Start(BattleEndCheckFlow check, BattleUnit unit)
    {
        this.unit = unit;
        this.check = check;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        onPhaseStartModify.Start(this, needSettle: true);
        foreach (var skillApplyFlow in onPhaseStartModify.skillApplyFlows)
        {
            flow.Append(skillApplyFlow.flow);
        }
        flow.Join(new UnitDefeatFlow().Start(unit).flow);
    }
}
