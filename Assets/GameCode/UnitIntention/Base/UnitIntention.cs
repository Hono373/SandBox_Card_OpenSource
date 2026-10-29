using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public abstract class UnitIntention
{
    [MinValue(0)] public int weight = 0;

    public abstract BattleUnitSkill GetResult(BattleUnit unit);
    public abstract UnitIntention Create();
    public virtual string Desc() => "";
    public IntentionCondition fliter = new();
    internal virtual void ValidateData() { }
    /// <summary>把基类公共字段搬给克隆体，各子类 Create 里必须调用，否则克隆后 weight/fliter 归零。</summary>
    protected void CopyBaseTo(UnitIntention target)
    {
        target.weight = weight;
        target.fliter = fliter;
    }
}
