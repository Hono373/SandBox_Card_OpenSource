using UnityEngine;

/// <summary>
/// 地狱之刃：随机塞一张攻击牌到手牌。
/// 池子 = 全卡表里 type==Attack 且 unplayable==false 的卡。
/// 「本回合耗能变为 0」没做 —— 项目没有「单张牌的临时费用覆盖」机制（BattleCardCostRule 只有 CostValue/CostX/CostNo 三种）。
/// </summary>
public class RandomAttackCardEffect : SkillEffect
{
    public override void ValidateData() { }
    public override string Desc() => "将一张随机攻击牌放入手牌";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new RandomAttackCardEffect() { hasSource = hasSource, effectAnimData = effectAnimData };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var pool = new System.Collections.Generic.List<string>();
        foreach (var info in GameManager.Instance.gameDB.cardInfos.Values)
        {
            if (info.graphic.type != TypeEnum.Attack) continue;
            if (info.unplayable) continue;
            pool.Add(info.key);
        }
        if (pool.Count == 0) return default;

        var deck = DeckBuildBattle.Active.deck;
        var key = pool[Random.Range(0, pool.Count)];
        var card = BattleCard.Create(key, deck.CreateCardId(), caster.data.id, true);
        deck.AddCard(card, PileEnum.Hand);
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new AddCardFlow().Start(PileEnum.Hand);
}
