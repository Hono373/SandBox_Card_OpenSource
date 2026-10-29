using System;

[Serializable]
public abstract class ModifierEffect
{
    public string desc;
    public abstract ModifierEffect Create();
    public virtual void Init(Modifier modifier) { }
    public virtual string GetDescription(Modifier modifier) => desc;
    public virtual void IsActive(Modifier modifier) { }
    public abstract void Modify(Modifier modifier, GameModify context, bool isPreModify);
    public virtual void ClearCounting(Modifier modifier) { }
    public virtual void Settle(Modifier modifier)
    {

    }
    internal virtual void ValidateData() { }
}
[Serializable]
public abstract class ModifierEffect<T> : ModifierEffect where T : GameModify
{
    public override void Init(Modifier modifier) { }
    public sealed override void Modify(Modifier modifier, GameModify context, bool needSettle = false)
    {
        if (context is T typed)
        {
            Modify(modifier, typed);
            // 只登记、不立即执行：Settle 会增删 modifiers，必须等 context 被消费完再统一跑
            if (needSettle)
                context.DeferSettle(() => Settle(modifier, typed));
        }
    }
    public abstract void Modify(Modifier modifier, T typed);
    public virtual void Settle(Modifier modifier, T context) { }
}
