public class PassiveModify_ModifierAdd : GameModify
{
    public ModifierAdd add;
    public BattleUnit caster;
    public BattleUnit target;
    public bool blocked;
    public PassiveModify_ModifierAdd() { }
    public PassiveModify_ModifierAdd Start(BattleUnit caster, BattleUnit target, ModifierAdd add, bool needSettle)
    {
        this.caster = caster;
        this.target = target;
        this.add = add;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
    }
}
