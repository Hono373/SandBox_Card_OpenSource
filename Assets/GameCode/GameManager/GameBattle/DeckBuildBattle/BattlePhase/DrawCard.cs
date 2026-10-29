public class DrawCard : GameModify
{
    public int drawNumbModifi;
    public int drawNumbBase;
    public bool isSkillEffect;
    public int drawNumbResult;
    public DrawCard Start(int count, bool isSkillEffect, bool needSettle)
    {
        drawNumbBase = count;
        this.isSkillEffect = isSkillEffect;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        drawNumbResult = (drawNumbBase + drawNumbModifi).GreaterThanOrEqual();
    }
}
