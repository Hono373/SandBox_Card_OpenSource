using System;

[Serializable]
public class CountXUseCost : CountRule
{
    public int count = 0;
    public override string Desc()
    {
        return "X次";
    }
    public override string IntentionDesc() => "";
    public override GameModify Modify(SkillApplyFlow phase, bool needSettle, out int countResult)
    {
        var modify = new BattleCardModify_CountX().Start(phase, this, true);
        countResult = modify.countResult;
        return modify;
    }
    public override CountRule Create() => new CountXUseCost() { count = DeckBuildBattle.Active.data.energy };
}
