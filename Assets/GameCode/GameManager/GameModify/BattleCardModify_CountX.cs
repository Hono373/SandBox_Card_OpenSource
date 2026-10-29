internal class BattleCardModify_CountX : GameModify
{
    public SkillApplyFlow skillphase;
    public CountXUseCost countX;
    public bool needSettle;
    public int countModifi;
    public int countResult;
    public BattleCardModify_CountX() { }
    public BattleCardModify_CountX Start(SkillApplyFlow phase, CountXUseCost countX, bool needSettle)
    {
        this.skillphase = phase;
        this.countX = countX;
        this.needSettle = needSettle;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        countResult = (skillphase.costUsed + countModifi).GreaterThanOrEqual(0);
    }
}
