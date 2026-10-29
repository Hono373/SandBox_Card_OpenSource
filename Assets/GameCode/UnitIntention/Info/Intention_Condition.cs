using System;

[Serializable]
public abstract class Intention_Condition
{
    /// <summary>返回 true 表示本节点通过筛选。unit 用于读取运行时状态（历史意图、buff、血量等）。</summary>
    public abstract bool Check(BattleUnit unit);
}
