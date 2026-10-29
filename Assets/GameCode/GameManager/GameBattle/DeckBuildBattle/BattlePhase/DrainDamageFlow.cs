using DG.Tweening;

/// <summary>
/// 死亡收割的表现：目标受击演出 + 自己回血的血条刷新。
/// 回血数值已经在 Apply 里算好并写进 data，这里只刷 UI（result.heal > 0 才刷）。
/// </summary>
public class DrainDamageFlow : BattleFlow
{
    public DrainDamageEffect effect;
    public BattleUnit caster;
    public BattleUnit target;
    public SkillEffectResult result;
    public DamageFlow damageFlow = new();

    public DrainDamageFlow Start(DrainDamageEffect effect, BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        this.effect = effect;
        this.caster = caster;
        this.target = target;
        this.result = result;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var ui = DeckBuildBattle.GetUI();
        var damage = new Damage { effectAnimData = effect.effectAnimData };
        flow.Join(damageFlow.Start(damage, caster, target, result).flow);

        if (result.heal <= 0) return;

        var casterUI = ui.unitContainer.GetUnitUI(caster);
        casterUI.hpSliderUI.RefreshHpBar(caster.data.hp, caster.data.maxHP, 0.2f, out var healRefresh);
        flow.Join(healRefresh);
    }
}
