using UnityEngine;

/// <summary>
/// 复制手牌中的一张攻击 / 能力牌，复制体放进手牌（双持）。
/// 选牌没有 UI，规则是「手牌里第一张 Attack 或 Ability」—— 比 STS 的玩家自选弱，
/// 但不需要额外的选牌界面。要真做自选得接 ChoiceFlow。
/// </summary>
public class CopyHandCardEffect : SkillEffect
{
    public override void ValidateData() { }
    public override string Desc() => "复制一张手牌中的攻击或能力牌";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new CopyHandCardEffect() { hasSource = hasSource, effectAnimData = effectAnimData };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var deck = DeckBuildBattle.Active.deck;
        BattleCard source = null;
        foreach (var card in deck.GetPile(PileEnum.Hand).list)
        {
            var type = CardInfo.Get(card.data.key).graphic.type;
            if (type != TypeEnum.Attack && type != TypeEnum.Ability) continue;
            source = card;
            break;
        }
        if (source == null) return default;

        var copy = BattleCard.Create(source.data.key, deck.CreateCardId(), source.data.owner, true);
        deck.AddCard(copy, PileEnum.Hand);
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new AddCardFlow().Start(PileEnum.Hand);
}
