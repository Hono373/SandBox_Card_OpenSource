public class PassiveModify_Shield : GameModify
{
    public Shield shield;
    public int valueModifi;
    public int? forceResult;
    public BattleUnit caster;
    public BattleUnit target;
    public PassiveModify_Shield() { }
    public PassiveModify_Shield Start(BattleUnit caster, BattleUnit target, Shield shield, bool needSettle)
    {
        this.caster = caster;
        this.target = target;
        this.shield = shield;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        shield.value = (shield.value - valueModifi).GreaterThanOrEqual(0);
        if (forceResult != null)
        {
            shield.value = forceResult.Value.GreaterThanOrEqual(0);
        }
    }
}
