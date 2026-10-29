using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 重振精神：消耗手牌中所有非 layer 类型的牌，每张获得 value 点格挡。
/// value 走 W 列、layer 走 Y 列（存 TypeEnum 数值，Attack=1 表示"消耗非攻击牌"）。
/// 挑牌在 Apply，移堆 + 动画 + 加格挡在 ExhaustHandShieldFlow。
/// </summary>
public class ExhaustHandShieldEffect : SkillEffect
{
    /// <summary>每张被消耗的牌提供的格挡（W 列）。</summary>
    [MinValue(0)] public int value;
    /// <summary>要排除的类型（Y 列，TypeEnum 数值）：等于该类型的牌留在手上。</summary>
    public TypeEnum layer = TypeEnum.Attack;

    /// <summary>本次被选中的手牌（Apply 写、Start 读）。</summary>
    [System.NonSerialized] public List<BattleCard> picked = new();

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"消耗手牌换格挡 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"消耗手牌中所有非{TypeName()}牌，每张获得{value}点格挡";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new ExhaustHandShieldEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value, layer = layer };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        foreach (var card in DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list)
        {
            if (CardInfo.Get(card.data.key).graphic.type == layer) continue;
            picked.Add(card);
        }
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new ExhaustHandShieldFlow().Start(picked, value, caster);

    string TypeName() => layer switch
    {
        TypeEnum.Attack => "攻击",
        TypeEnum.Skill => "技能",
        TypeEnum.Ability => "能力",
        _ => "牌",
    };
}
