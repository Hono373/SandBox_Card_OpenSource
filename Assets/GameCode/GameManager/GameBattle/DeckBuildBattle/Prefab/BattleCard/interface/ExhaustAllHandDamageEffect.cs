using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 恶魔之焰：消耗**所有**手牌，每张对被指定的目标造成 value 点伤害（全部结算在同一个 effect 里，
/// 因为伤害总量 = 手牌数 × value，拆成两个 skill 会依赖执行顺序）。
/// value 走 W 列。挑牌在 Apply、移堆与动画在 ExhaustAllHandDamageFlow。
/// </summary>
public class ExhaustAllHandDamageEffect : SkillEffect
{
    /// <summary>每张被消耗的牌造成的伤害（W 列）。</summary>
    [MinValue(0)] public int value;

    /// <summary>本次被选中的手牌（Apply 写、Start 读）。</summary>
    [System.NonSerialized] public List<BattleCard> picked = new();

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"消耗手牌打伤害 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"消耗所有手牌，每张造成{value}点伤害";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new ExhaustAllHandDamageEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        picked.AddRange(DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list);

        var total = picked.Count * value;
        var overflow = target.data.shield - total;
        if (overflow >= 0)
        {
            target.data.shield = overflow;
            return new SkillEffectResult(damage: 0, heal: 0, shieldLost: total, string.Empty);
        }

        var shieldAbsorb = target.data.shield;
        // 2026-10-22 飘字/吸血用「格挡后的真实伤害」realDamage，不再以目标剩余血量为上限
        var realDamage = -overflow;
        var hpAfter = Mathf.Max(0, target.data.hp - realDamage);
        target.data.shield = 0;
        target.data.hp = hpAfter;
        return new SkillEffectResult(damage: realDamage, heal: 0, shieldLost: shieldAbsorb, string.Empty);
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new ExhaustAllHandDamageFlow().Start(picked, caster, target, result, effectAnimData);
}
