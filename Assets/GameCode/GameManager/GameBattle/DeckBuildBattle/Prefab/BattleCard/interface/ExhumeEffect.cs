using UnityEngine;

/// <summary>
/// 发掘：从消耗堆（PileEnum.Removed）取回一张牌放进手牌，卡本身也消耗。
/// 取「最早进堆的那张」（Index 0），比 STS 的玩家自选弱，但不需要额外的选牌界面。
/// </summary>
public class ExhumeEffect : SkillEffect
{
    /// <summary>本次取回的牌（Apply 写、Start 读）。</summary>
    [System.NonSerialized] public BattleCard picked;

    public override void ValidateData() { }
    public override string Desc() => "从消耗堆取回一张牌";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new ExhumeEffect() { hasSource = hasSource, effectAnimData = effectAnimData };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var deck = DeckBuildBattle.Active.deck;
        var exhaustPile = deck.GetPile(PileEnum.Removed);
        if (exhaustPile.Count() == 0) return default;

        picked = exhaustPile.Index(0);
        exhaustPile.Remove(picked);
        deck.AddTo(PileEnum.Hand, picked);
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new ExhumeFlow().Start(picked);
}
