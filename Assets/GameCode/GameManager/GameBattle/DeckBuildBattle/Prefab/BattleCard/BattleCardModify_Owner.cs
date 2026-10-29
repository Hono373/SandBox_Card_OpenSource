using System;

[Serializable]
public class BattleCardModify_Owner : GameModify
{
    public BattleCardModified modified;
    public BattleUnit ownerResult;
    public BattleCardModify_Owner Start(BattleCardModified modified, bool needSettle)
    {
        this.modified = modified;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        ownerResult = DeckBuildBattle.Active.data.unitDict.GetBattleUnitById(modified.Card.data.owner);
    }
}
