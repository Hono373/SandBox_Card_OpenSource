using DG.Tweening;
public class HealFlow : BattleFlow
{
    public Heal heal;
    public BattleUnit caster;
    public BattleUnit target;
    public SkillEffectResult result;

    internal BattleFlow Start(Heal skill, BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        this.heal = skill;
        this.caster = caster;
        this.target = target;
        this.result = result;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {

        var targetUI = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(target);
        targetUI.hpSliderUI.RefreshHpBar(target.data.hp, target.data.maxHP, 0.2f, out var hpRefresh);
        DeckBuildBattle.GetUI().popValueContainer.PopValue(targetUI, result, out var valuePopAnim);

        flow.Join(valuePopAnim);
        flow.Join(hpRefresh);
        flow.Join(EffectAnimObj.CreateAnim(heal.effectAnimData).TargetAnim(target));
    }
}