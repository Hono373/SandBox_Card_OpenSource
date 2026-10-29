using System;

/// <summary>易伤：受到的伤害增加 50%</summary>
[Serializable]
public class VulnerableEffect : ModifierEffect<PassiveModify_Damage>
{
    public override ModifierEffect Create() => new VulnerableEffect();
    public override void Modify(Modifier modifier, PassiveModify_Damage context)
    {
        if (context.target != modifier.GetOwner()) return;
        // 2026-10-23 hasSource 修好后这个判断才真正生效：无来源伤害（自伤）不吃易伤。
        if (context.damage.hasSource)
            context.percentModifi += 50;
    }
}
