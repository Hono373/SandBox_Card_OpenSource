using UnityEngine;

/// <summary>
/// 把某个牌堆顶部的牌挪到另一个牌堆（头槌：弃牌堆顶 → 抽牌堆顶；战吼：手牌 → 抽牌堆顶）。
/// value 走 W 列 = 源牌堆，layer 走 Y 列 = 目标牌堆（都存 PileEnum 数值）。
/// 只动数据，UI 数字靠 AddCardFlow 刷新。
/// </summary>
public class MoveTopCardEffect : SkillEffect
{
    /// <summary>源牌堆（W 列）。</summary>
    public PileEnum value = PileEnum.Discard;
    /// <summary>目标牌堆（Y 列）。</summary>
    public PileEnum layer = PileEnum.Draw;

    public override void ValidateData()
    {
        if (value == layer)
            Debug.LogError($"挪牌 {GetType().Name} 源堆与目标堆相同（{value}）");
    }
    public override string Desc() => $"将{value}顶部的一张牌移到{layer}";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new MoveTopCardEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value, layer = layer };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var from = DeckBuildBattle.Active.deck.GetPile(value);
        var to = DeckBuildBattle.Active.deck.GetPile(layer);
        if (from.Count() == 0) return default;

        var card = from.Index(0);
        from.Remove(card);
        to.Add(card);
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new AddCardFlow().Start(layer);
}
