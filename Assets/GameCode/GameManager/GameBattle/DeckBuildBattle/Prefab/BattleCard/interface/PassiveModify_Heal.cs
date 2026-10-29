public class PassiveModify_Heal : GameModify
{
    public Heal heal;
    public int valueModifi;
    public int? forceResult;
    public BattleUnit caster;
    public BattleUnit target;
    public PassiveModify_Heal() { }
    public PassiveModify_Heal Start(BattleUnit caster, BattleUnit target, Heal heal, bool needSettle)
    {
        this.caster = caster;
        this.target = target;
        this.heal = heal;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        heal.value = (heal.value - valueModifi).GreaterThanOrEqual(0);
        if (forceResult != null)
        {
            heal.value = forceResult.Value.GreaterThanOrEqual(0);
        }
    }
}
