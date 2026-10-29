using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 塞牌：把指定卡牌放入牌堆（状态牌、复制品等）。
///
/// 字段与表列的对应关系（effect 子字段只能在 U~AC 内，字段名必须等于表头名）：
///   key   -> X(24)  string  要放入的卡牌 key
///   value -> W(23)  int     放入数量
///   layer -> Y(25)  int     目标牌堆（PileEnum 数值：1=TotalPile 2=Draw 3=Hand 4=Discard 5=Removed 6=Destroy）
/// 也就是说 value / layer 在这里的语义不是「数值」和「层数」，而是「数量」和「目标堆」——
/// 这是 effect 列复用的既定代价，按 $type 解释。
/// </summary>
public class AddCardEffect : SkillEffect
{
    /// <summary>要放入牌堆的卡牌 key（X 列）。</summary>
    public string key;
    /// <summary>放入数量（W 列 value）。</summary>
    [MinValue(1)] public int value = 1;
    /// <summary>目标牌堆（Y 列 layer，存 PileEnum 数值）。</summary>
    public PileEnum layer = PileEnum.Draw;

    public override void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(key))
            Debug.LogError($"塞牌 {GetType().Name} key 为空");
        if (value <= 0)
            Debug.LogError($"塞牌 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"将{value}张{key}放入{layer}";
    public override string IntentionDesc() => $"塞牌{value}";
    public override SkillEffect Create() => new AddCardEffect() { hasSource = hasSource, effectAnimData = effectAnimData, key = key, value = value, layer = layer };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var deck = DeckBuildBattle.Active.deck;
        for (int i = 0; i < value; i++)
        {
            var card = BattleCard.Create(key, deck.CreateCardId(), target.data.id, true);
            deck.AddCard(card, layer);
        }
        return default;
    }
    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        return new AddCardFlow().Start(layer);
    }
}
