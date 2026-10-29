using System;

internal class BattleCardModify_UnderSkill : GameModify
{
    public BattleCardModified current;
    public BattleUnit target;
    public Damage skillEffect;
    public int valueModifi;
    public int percentModifi;
    public SortedWeight<int?> forceModifiWeight = new();
    public BattleCardModify_UnderSkill Start(BattleCardModified current, BattleUnit target, Damage commonSkill, bool needSettle)
    {
        this.skillEffect = commonSkill;
        this.target = target;
        return Start(current, needSettle);
    }
    public BattleCardModify_UnderSkill Start(BattleCardModified current, bool needSettle)
    {
        this.current = current;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        var baseValue = skillEffect.value;
        var resultPrecent = (100 - percentModifi).GreaterThanOrEqual();
        var result = (baseValue * resultPrecent / 100 - valueModifi).GreaterThanOrEqual();

        int? forceResult = forceModifiWeight.MaxOrDefault(() => null);
        if (forceResult.HasValue)
        {
            result = forceResult.Value;
        }
        skillEffect.value = result;
    }
}
