using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 随机消耗手牌（坚毅的「消耗一张随机手牌」）。
/// 只挑牌、不移牌堆 —— 移堆和消失动画都在 ExhaustFlow 里做，避免 flow 没跑时手牌凭空少卡。
/// picked 用 [NonSerialized]：Apply 挑选、Start 读取，同一个 effect 实例传递。
/// </summary>
public class ExhaustRandomHandEffect : SkillEffect
{
    /// <summary>随机消耗几张（W 列）。</summary>
    [MinValue(1)] public int value = 1;

    /// <summary>本次被选中的手牌（Apply 写、Start 读）。</summary>
    [System.NonSerialized] public List<BattleCard> picked = new();

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"随机消耗 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"消耗{value}张随机手牌";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new ExhaustRandomHandEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var hand = DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list;
        var pool = new List<BattleCard>(hand);
        for (int i = 0; i < value && pool.Count > 0; i++)
        {
            var index = Random.Range(0, pool.Count);
            picked.Add(pool[index]);
            pool.RemoveAt(index);
        }
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new ExhaustFlow().Start(picked);
}
