using DG.Tweening;

public class ShieldFlow : BattleFlow
{
    public Shield shield;
    public BattleUnit caster;
    public BattleUnit target;
    public ShieldFlow Start(Shield shield, BattleUnit caster, BattleUnit target)
    {
        this.shield = shield;
        this.caster = caster;
        this.target = target;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var targetUI = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(target);
        flow.Join(targetUI.shieldUI.Refresh());
        flow.Join(EffectAnimObj.CreateAnim(shield.effectAnimData).TargetAnim(target));
    }
}
