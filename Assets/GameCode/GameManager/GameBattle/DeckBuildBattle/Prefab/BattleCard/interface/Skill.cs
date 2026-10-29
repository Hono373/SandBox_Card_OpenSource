using System;
using UnityEngine;
[Serializable]
public class Skill
{
    public RangeRule rangeRule = new();
    [SerializeReference] public CountRule countRule = new VariableCount();
    [SerializeReference] public SkillEffect effect;
    public Skill Create()
    {
        return new Skill()
        {
            rangeRule = rangeRule.Create(),
            effect = effect.Create(),
            countRule = countRule.Create(),
        };
    }
    public string Desc() => string.Concat(effect.Desc(), countRule.Desc());
    public string IntentionDesc()
    {
        if (effect.IntentionDesc() == "")
            return "";
        if (countRule.IntentionDesc() == "")
            return effect.IntentionDesc();
        return string.Concat(effect.IntentionDesc(), "X", countRule.IntentionDesc());
    }
    internal virtual void ValidateData()
    {
        if (rangeRule == null)
            Debug.LogError($"技能 {GetType().Name} range 为空");
        if (effect == null)
            Debug.LogError($"技能 {GetType().Name} effect 为空");
        if (countRule == null)
            Debug.LogError($"技能 {GetType().Name} countMode 为空");
    }
}
