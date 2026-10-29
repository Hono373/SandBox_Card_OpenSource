using System;

/// <summary>脆弱：获得的格挡减少 25%</summary>
[Serializable]
public class FrailEffect : ModifierEffect<ActiveModify_Shield>
{
    public override ModifierEffect Create() => new FrailEffect();
    public override void Modify(Modifier modifier, ActiveModify_Shield context)
    {
        if (context.caster == modifier.GetOwner())
            context.percentModifi -= 25;
    }
}
