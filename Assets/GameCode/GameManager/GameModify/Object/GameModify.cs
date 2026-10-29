using System;
using System.Collections.Generic;
[Serializable]
public abstract class GameModify
{
    /// <summary>Modify 阶段登记的延迟结算：消费方读完 context 之后统一执行，避免遍历 modifiers 时增删集合</summary>
    [NonSerialized] readonly List<Action> settles = new();
    internal void DeferSettle(Action settle) => settles.Add(settle);
    /// <summary>context 被消费（Apply/CreateResult）之后调用，执行本轮登记的 Settle（扣层、移除、加力量等）</summary>
    public void Settle()
    {
        if (settles.Count == 0) return;
        var snapshot = new List<Action>(settles);
        settles.Clear();
        foreach (var settle in snapshot) settle();
    }
    protected void ModifyAndCreateResult(bool needSettle)
    {
        GameModifyCenter.Modify(this, needSettle);
        CreateResult();
    }
    protected virtual void CreateResult() { }
}
