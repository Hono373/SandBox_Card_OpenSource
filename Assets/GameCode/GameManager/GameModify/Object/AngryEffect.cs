using System;
using DG.Tweening;

/// <summary>生气：受到攻击伤害后获得与层数相等的力量（原版 Gremlin 的 Angry 被动）</summary>
[Serializable]
public class AngryEffect : ModifierEffect<AfterTakeDmg>
{
    public override ModifierEffect Create() => new AngryEffect();

    /// <summary>不改任何结算数值，只在伤害落地后加力量，所以 Modify 留空、副作用写在 Settle 里（needSettle 时才执行）。</summary>
    public override void Modify(Modifier modifier, AfterTakeDmg context) { }

    public override void Settle(Modifier modifier, AfterTakeDmg context)
    {
        var owner = modifier.GetOwner();
        if (context.target != owner) return;
        var add = new ModifierAdd { key = "Strength", layer = modifier.data.layer };
        var result = add.Apply(owner, owner, null);
        // 2026-10-21 必须走 ModifierAddFlow：它负责刷 buff 图标，并按新力量重算敌方意图数值（不重抽意图）。
        new ModifierAddFlow().Start(add, owner, owner, result).flow.Play();
    }
}
