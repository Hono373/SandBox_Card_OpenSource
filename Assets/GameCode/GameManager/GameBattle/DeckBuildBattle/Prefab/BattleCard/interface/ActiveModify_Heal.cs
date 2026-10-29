public class ActiveModify_Heal : GameModify
{
    public Heal heal;
    public int valueModifi;
    public int? forceResult;
    public BattleUnit caster;
    public ActiveModify_Heal() { }
    public ActiveModify_Heal Start(BattleUnit caster, Heal heal, bool needSettle)
    {
        this.caster = caster;
        this.heal = heal;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        heal.value = (heal.value + valueModifi).GreaterThanOrEqual(0);
        if (forceResult != null)
        {
            heal.value = forceResult.Value.GreaterThanOrEqual(0);
        }
    }
}
