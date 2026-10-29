using System;

[Serializable]
public class BattleCardModify_Useable : GameModify
{
    public BattleCardModified current;
    public bool useBlockModify;
    public bool useBlockResult;
    public BattleCardModify_Useable() { }
    public BattleCardModify_Useable Start(BattleCardModified current, bool needSettle)
    {
        this.current = current;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        if (current.Info.unplayable)
        {
            useBlockResult = true;
            return;
        }
        if (current.costResult > DeckBuildBattle.Active.data.energy)
        {
            useBlockResult = true;
            return;
        }
        if (useBlockModify)
        {
            useBlockResult = useBlockModify;
            return;
        }
    }
}
