public class PassiveModify_Damage : GameModify
{
    public Damage damage;
    public int valueModifi;
    public int percentModifi;
    public int? forceResult;
    public BattleUnit caster;
    public BattleUnit target;
    public PassiveModify_Damage() { }
    public PassiveModify_Damage Start(BattleUnit caster, BattleUnit target, Damage damage, bool needSettle)
    {
        this.caster = caster;
        this.target = target;
        this.damage = damage;
        // 2026-10-23 明细记录：同上，进本阶段前的值必须在 CreateResult 之前取
        int baseValue = damage.value;
        ModifyAndCreateResult(needSettle);
        // 2026-10-23 明细记录：目标侧落定时合成一条「谁打谁、中间改了几次、实际多少」
        // 目标侧公式是 (value - valueModifi)，所以定点增量记为负的 valueModifi
        if (needSettle)
            DamageBreakdown.CompleteTarget(caster.Info.unitName, target.Info.unitName, baseValue,
                                           -valueModifi, percentModifi, forceResult != null, damage.value);
        return this;
    }
    protected override void CreateResult()
    {
        damage.value = (damage.value - valueModifi).GreaterThanOrEqual(0);
        damage.value = damage.value * (100 + percentModifi) / 100;
        if (forceResult != null)
        {
            damage.value = forceResult.Value.GreaterThanOrEqual(0);
        }
    }
}
