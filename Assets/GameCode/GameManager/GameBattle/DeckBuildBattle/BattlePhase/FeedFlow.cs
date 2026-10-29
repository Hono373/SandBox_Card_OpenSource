using DG.Tweening;

/// <summary>
/// 狂宴的表现流程：目标受击演出 + 击杀时给自己加最大生命。
///
/// 伤害演出照抄 DamageFlow（FeedEffect 不是 Damage，没法复用那个类的 Start 签名），
/// 击杀加成放在这里是因为它需要「数据 + 血条」一起变 —— 数据在 effect 自己的 FlowBuild 里改，
/// 符合「FlowBuild 是唯一入口」的约定。
/// </summary>
public class FeedFlow : BattleFlow
{
    public FeedEffect effect;
    public BattleUnit caster;
    public BattleUnit target;
    public SkillEffectResult result;

    public FeedFlow Start(FeedEffect effect, BattleUnit caster, BattleUnit target, SkillEffectResult result)
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
        var targetUI = ui.unitContainer.GetUnitUI(target);

        targetUI.skeleton.UnderAttackAnim(out var underAtkAnim);
        targetUI.hpSliderUI.RefreshHpBar(target.data.hp, target.data.maxHP, 0.2f, out var hpRefresh);

        flow.Join(hpRefresh);
        flow.Join(targetUI.shieldUI.Refresh());
        flow.Join(underAtkAnim);
        flow.Join(EffectAnimObj.CreateAnim(effect.effectAnimData).TargetAnim(target));
        ui.popValueContainer.PopValue(targetUI, result, out var popValue);
        flow.Join(popValue);

        if (!effect.killed) return;

        // 2026-10-22 斩杀加成：双写。
        // BattleUnitData = 本场战斗的实际数值（血条、后续结算都读它）；
        // Unit.data      = 存档级持久值（GameSerialize.Active.unitLibrary），下场战斗 Create 时拷过去。
        // 只写前者 = 打完这场就没了；只写后者 = 血条不涨。
        caster.data.maxHP += effect.layer;
        caster.data.hp += effect.layer;
        var persist = GameSerialize.Active.unitLibrary.GetUnitById(caster.data.id);
        persist.data.maxHP += effect.layer;
        persist.data.hp += effect.layer;

        var casterUI = ui.unitContainer.GetUnitUI(caster);
        casterUI.hpSliderUI.RefreshHpBar(caster.data.hp, caster.data.maxHP, 0.2f, out var casterHpRefresh);
        flow.Append(casterHpRefresh);
    }
}
