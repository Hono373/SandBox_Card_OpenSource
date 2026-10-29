using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public abstract class SkillEffectInfo_Ob : ISkillEffectInfo_Ob
{
    public int value;
    public int bonus;
    public string desc;
    public EffectIconEnum effectType;
    public abstract void OneFrequency(BattleUnit source, List<BattleUnit> targets, out Sequence oneFrequency);
    public virtual IntentionUIData_Ob IntentionsData()
    {

        return new IntentionUIData_Ob(effectType, value, bonus);
    }
    public abstract string Desc(BattleUnit source, BattleUnit target);

}