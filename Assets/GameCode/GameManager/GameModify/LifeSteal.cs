using System;
using System.Collections.Generic;
using DG.Tweening;
public class LifeSteal : GameTag<OnPhaseStartModify>
{
    public override void Apply(OnPhaseStartModify skillPhase)
    {
        if (skillPhase.phase is DamageFlow flow)
        {
            
        }
    }
}
