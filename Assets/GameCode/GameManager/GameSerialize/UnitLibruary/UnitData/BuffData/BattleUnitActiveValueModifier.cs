public class BattleUnitActiveValueModifier
{
    BaseValueModifier baseValueModifier;
    public BattleUnit source;
    public BattleUnitActiveValueModifier() { }
    public BattleUnitActiveValueModifier(BattleUnit source) { this.source = source; }
    public int CalculateModifiedResult(int baseValue)
    {
        return baseValueModifier.CalculateModifiedResult(baseValue);
    }
}

