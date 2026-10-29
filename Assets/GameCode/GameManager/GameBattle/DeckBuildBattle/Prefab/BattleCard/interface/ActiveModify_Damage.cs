public class ActiveModify_Damage : GameModify
{
    public Damage damage;
    public int valueModifi;
    public int percentModifi;
    public int? forceResult;
    public BattleUnit caster;
    public ActiveModify_Damage() { }
    public ActiveModify_Damage Start(BattleUnit caster, Damage damage, bool needSettle)
    {
        this.caster = caster;
        this.damage = damage;
        // 2026-10-23 明细记录：进本阶段前的值必须在 CreateResult 之前取，CreateResult 是原地改写
        int baseValue = damage.value;
        ModifyAndCreateResult(needSettle);
        // 2026-10-23 明细记录：只暂存，等目标侧落定后合成一条（否则一次伤害会显示两行）
        // needSettle:false 是预演（卡面数字），不记，否则每条伤害会被记两遍
        if (needSettle)
            DamageBreakdown.BeginCaster(caster.Info.unitName, baseValue, valueModifi);
        return this;
    }
    protected override void CreateResult()
    {
        damage.value = (damage.value + valueModifi).GreaterThanOrEqual(0);
        damage.value = damage.value * (100 + percentModifi) / 100;
        if (forceResult != null)
        {
            damage.value = forceResult.Value.GreaterThanOrEqual(0);
        }
    }
}
