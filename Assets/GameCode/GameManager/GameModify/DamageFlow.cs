using System;
using DG.Tweening;

public class DamageFlow : BattleFlow
{
    public Damage damage;
    public BattleUnit caster;
    public BattleUnit target;
    public SkillEffectResult result;
    public DamageFlow Start(Damage damage, BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        this.damage = damage;
        this.caster = caster;
        this.target = target;
        this.result = result;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var targetUI = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(target);

        targetUI.skeleton.UnderAttackAnim(out var underAtkAnim);
        targetUI.hpSliderUI.RefreshHpBar(target.data.hp, target.data.maxHP, 0.2f, out var hpRefresh);

        flow.Join(hpRefresh);
        flow.Join(targetUI.shieldUI.Refresh());
        flow.Join(underAtkAnim);
        flow.Join(EffectAnimObj.CreateAnim(damage.effectAnimData).TargetAnim(target));

        DeckBuildBattle.GetUI().popValueContainer.PopValue(targetUI, result, out var popValue);
        flow.Join(popValue);
    }
}
