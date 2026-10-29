using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 恶魔之焰的表现流程：交给 ExhaustFlow 烧手牌，然后接伤害演出。
/// 伤害不在这里算（Apply 已经算完并给了 result），这里只负责播。
/// </summary>
public class ExhaustAllHandDamageFlow : BattleFlow
{
    public List<BattleCard> exhausts = new();
    public BattleUnit caster;
    public BattleUnit target;
    public SkillEffectResult result;
    public EffectAnimData effectAnimData;
    public ExhaustFlow exhaustFlow = new();
    public DamageFlow damageFlow = new();

    public ExhaustAllHandDamageFlow Start(List<BattleCard> cards, BattleUnit caster, BattleUnit target,
                                          SkillEffectResult result, EffectAnimData effectAnimData)
    {
        exhausts.AddRange(cards);
        this.caster = caster;
        this.target = target;
        this.result = result;
        this.effectAnimData = effectAnimData;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        flow.Join(exhaustFlow.Start(exhausts).flow);

        // 手牌烧完之后再接伤害演出
        var damage = new Damage { effectAnimData = effectAnimData };
        flow.Append(damageFlow.Start(damage, caster, target, result).flow);
    }
}
