public class ActiveModify_ModifierAdd : GameModify
{
    public ModifierAdd add;
    public BattleUnit caster;
    public ActiveModify_ModifierAdd() { }
    public ActiveModify_ModifierAdd Start(BattleUnit caster, ModifierAdd add, bool needSettle)
    {
        this.caster = caster;
        this.add = add;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
    }
}
