using System;

/// <summary>力量：造成的伤害增加，数值等于层数</summary>
[Serializable]
public class StrengthEffect : ModifierEffect<ActiveModify_Damage>
{
    public override ModifierEffect Create() => new StrengthEffect();
    public override void Modify(Modifier modifier, ActiveModify_Damage context)
    {
        if (context.caster != modifier.GetOwner()) return;
        // 2026-10-23 无来源伤害（hasSource=false）= 祭品/放血这类「失去 N 点生命」，数值固定，
        // 不是「造成的伤害」，力量不该加成。
        if (!context.damage.hasSource) return;
        context.valueModifi += modifier.data.layer;
    }
}
