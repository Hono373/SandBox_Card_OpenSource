using UnityEngine;

/// <summary>
/// 消耗手牌：把手里「除 value 以外」的牌全部移入消耗堆（PileEnum.Removed）。
/// 断魂斩 = value: Attack（消耗所有非攻击牌）。
///
/// 字段名只能叫 value：Luban 表里 effect 的子字段被限制在表头合并区 U~AC 内，
/// 新增列（AD 之后）属于 CardInfo 顶层字段，effect 子类读不到。
/// 所以复用 W 列（effect 主参数），跟 Damage/Heal/Shield 共用一个位置 ——
/// 本类的 value 语义是 TypeEnum 的数值（0=None 1=Attack 2=Skill 3=Ability），不是数值量。
///
/// 数据改动与消失动画全部在 ExhaustFlow 里做（跟 DrawCardEffect → DrawCardFlow 同一路数）：
/// Apply 只返回 default，不提前改牌堆 —— 否则 flow 没跑时手牌会凭空少卡。
/// </summary>
public class ExhaustHandEffect : SkillEffect
{
    /// <summary>要排除的类型（不消耗）：手牌里等于该类型的牌留下，其余全部消耗。</summary>
    public TypeEnum value = TypeEnum.Attack;

    public override void ValidateData()
    {
        if (value == TypeEnum.None)
            Debug.LogError($"消耗手牌 {GetType().Name} value 为 None，会把整手牌都消耗掉");
    }
    public override string Desc() => $"消耗手牌中所有非{TypeName()}牌";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new ExhaustHandEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify) => default;

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new ExhaustFlow().Start(PileEnum.Hand, card => CardInfo.Get(card.data.key).graphic.type != value);

    string TypeName() => value switch
    {
        TypeEnum.Attack => "攻击",
        TypeEnum.Skill => "技能",
        TypeEnum.Ability => "能力",
        _ => "牌",
    };
}
