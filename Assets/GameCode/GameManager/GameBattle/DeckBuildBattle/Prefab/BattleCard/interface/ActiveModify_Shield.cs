public class ActiveModify_Shield : GameModify
{
    public Shield shield;
    public int valueModifi;
    public int percentModifi;
    public int? forceResult;
    public BattleUnit caster;
    public ActiveModify_Shield() { }
    public ActiveModify_Shield Start(BattleUnit caster, Shield shield, bool needSettle)
    {
        this.caster = caster;
        this.shield = shield;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        shield.value = (shield.value + valueModifi).GreaterThanOrEqual(0);
        shield.value = shield.value * (100 + percentModifi) / 100;
        if (forceResult != null)
        {
            shield.value = forceResult.Value.GreaterThanOrEqual(0);
        }
    }
}
