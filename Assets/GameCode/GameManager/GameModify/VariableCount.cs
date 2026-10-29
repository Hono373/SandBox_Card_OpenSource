using System;
using Sirenix.OdinInspector;

[Serializable]
public class VariableCount : CountRule
{
    [MinValue(1)] public int count = 1;
    public override CountRule Create() => new VariableCount() { count = count };
    public override string Desc()
    {
        if (count > 1)
        {
            return string.Concat(count, "次");
        }
        return string.Empty;
    }
    public override string IntentionDesc()
    {
        if (count > 1)
        {
            return $"{count}";
        }
        return string.Empty;
    }
    public override GameModify Modify(SkillApplyFlow phase, bool needSettle, out int countResult)
    {
        countResult = count;
        return null;
    }
}
