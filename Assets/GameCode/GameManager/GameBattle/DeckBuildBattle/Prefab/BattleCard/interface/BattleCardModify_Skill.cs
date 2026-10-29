public class BattleCardModify_Skill : GameModify
{
    public bool needSettle;
    public BattleCardModified modified;
    public Skill skill;
    public GameModify modify;
    public BattleCardModify_Skill Start(BattleCardModified modified, Skill skill, bool needSettle)
    {
        this.modified = modified;
        this.skill = skill;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        modify = skill.effect.ActiveModify(modified.owner, needSettle);
        // ActiveModify 内部已把结果写回技能副本，这里再结算 modifier 自身状态
        modify.Settle();
    }
}
