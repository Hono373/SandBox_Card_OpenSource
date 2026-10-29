using UnityEngine;

/// <summary>
/// 战吼：把手里第一张牌放到抽牌堆顶（STS 是玩家自选，这里取第一张，省掉选牌界面）。
/// 数据在 Apply 里搬，UI 销毁与重排在 MoveHandTopToDrawFlow。
/// </summary>
public class MoveHandTopToDrawEffect : SkillEffect
{
    /// <summary>本次被移走的牌（Apply 写、Start 读）。</summary>
    [System.NonSerialized] public BattleCard picked;

    public override void ValidateData() { }
    public override string Desc() => "将一张手牌放到抽牌堆顶部";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new MoveHandTopToDrawEffect() { hasSource = hasSource, effectAnimData = effectAnimData };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var deck = DeckBuildBattle.Active.deck;
        var handPile = deck.GetPile(PileEnum.Hand);
        if (handPile.Count() == 0) return default;

        picked = handPile.Index(0);
        handPile.Remove(picked);
        // 抽牌堆顶 = list[0]（DeckBuildBattleDeck.Draw 从 Index(0) 取）
        deck.GetPile(PileEnum.Draw).list.Insert(0, picked);
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new MoveHandTopToDrawFlow().Start(picked);
}
