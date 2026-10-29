using Sirenix.OdinInspector;
using UnityEngine;

public abstract class GameTag
{
    public string desc;
    public abstract void Settle(GameModify context);
}

public abstract class GameTag<T> : GameTag where T : GameModify
{
    public sealed override void Settle(GameModify context)
    {
        if (context is T typed)
            Apply(typed);
    }
    public abstract void Apply(T context);
}
