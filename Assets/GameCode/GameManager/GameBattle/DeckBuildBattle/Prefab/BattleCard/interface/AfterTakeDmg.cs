public class AfterTakeDmg : GameModify
{
    /// <summary>造成这次伤害的单位。</summary>
    public BattleUnit caster;
    /// <summary>受伤的单位（可能已死亡）。</summary>
    public BattleUnit target;
    /// <summary>实际扣除的血量；护盾全部吸收时为 0。</summary>
    public int damage;

    public AfterTakeDmg Start(BattleUnit caster, BattleUnit target, int damage, bool needSettle)
    {
        this.caster = caster;
        this.target = target;
        this.damage = damage;
        ModifyAndCreateResult(needSettle);
        // 该事件本身就是"受伤后触发"，没有后续消费方，这里就是结算点
        Settle();
        return this;
    }
}
