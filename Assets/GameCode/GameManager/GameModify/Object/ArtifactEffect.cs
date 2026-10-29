using System;

/// <summary>人工制品：抵挡一次 debuff</summary>
[Serializable]
public class ArtifactEffect : ModifierEffect<PassiveModify_ModifierAdd>
{
    public override ModifierEffect Create() => new ArtifactEffect();
    public override void Modify(Modifier modifier, PassiveModify_ModifierAdd context)
    {
        if (modifier.data.layer <= 0)
            return;
        if (context.target != modifier.GetOwner())
            return;
        if (!ModifierInfo.Get(context.add.key).debuff)
            return;
        context.blocked = true;
    }
    public override void Settle(Modifier modifier, PassiveModify_ModifierAdd context)
    {
        if (!context.blocked)
            return;
        modifier.data.layer--;
        if (modifier.data.layer > 0)
            return;
        modifier.GetOwner().data.modifiers.Remove(modifier);
        GameModifyCenter.RemoveRegister(modifier);
    }
}
