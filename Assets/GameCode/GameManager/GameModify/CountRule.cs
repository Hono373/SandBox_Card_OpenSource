using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[Serializable]
public abstract class CountRule
{
    public abstract CountRule Create();
    public abstract GameModify Modify(SkillApplyFlow phase, bool needSettle, out int countResult);
    public abstract string Desc();
    public abstract string IntentionDesc();
}

