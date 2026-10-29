using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 完美打击：造成 value 点伤害，本场卡组里每有一张 key 含 layerName 的牌，伤害 +layer。
/// 统计范围用 TotalPile（本场全卡总表，含手牌/抽牌堆/弃牌堆/消耗堆），含自己。
/// value 走 W 列、layer 走 Y 列、layerName 走 X 列（默认 "Strike"）。
/// </summary>
public class DamagePerKeyEffect : SkillEffect
{
    /// <summary>基础伤害（W 列）。</summary>
    [MinValue(1)] public int value;
    /// <summary>每张匹配牌的加成（Y 列）。</summary>
    [MinValue(1)] public int layer = 2;
    /// <summary>匹配的 key 片段（X 列）。</summary>
    public string key = "Strike";

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"按牌名加成 {GetType().Name} value 小于等于 0");
        if (string.IsNullOrWhiteSpace(key))
            Debug.LogError($"按牌名加成 {GetType().Name} key 为空");
    }
    public override string Desc() => $"造成{value}点伤害，每有一张{key}牌伤害+{layer}";
    public override string IntentionDesc() => $"{value}";
    public override SkillEffect Create() => new DamagePerKeyEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value, layer = layer, key = key };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var matched = 0;
        foreach (var card in DeckBuildBattle.Active.deck.GetPile(PileEnum.TotalPile).list)
        {
            if (card.data.key.Contains(key)) matched++;
        }

        var total = value + matched * layer;
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
        => new DamageFlow().Start(new Damage { effectAnimData = effectAnimData }, caster, target, result);
}
