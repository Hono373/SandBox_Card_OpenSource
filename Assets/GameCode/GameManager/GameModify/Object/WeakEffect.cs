using System;

/// <summary>虚弱：造成的伤害减少 25%</summary>
[Serializable]
public class WeakEffect : ModifierEffect<ActiveModify_Damage>
{
    public override ModifierEffect Create() => new WeakEffect();
    public override void Modify(Modifier modifier, ActiveModify_Damage context)
    {
        if (context.caster != modifier.GetOwner()) return;
        // 2026-10-23 hasSource 原来自 Create() 起就丢了、恒为 true，这个判断等于没写；
        // 修好 Create 后它才真正生效：无来源伤害（自伤）不吃虚弱。
        if (context.damage.hasSource)
            context.percentModifi -= 25;
    }
}
