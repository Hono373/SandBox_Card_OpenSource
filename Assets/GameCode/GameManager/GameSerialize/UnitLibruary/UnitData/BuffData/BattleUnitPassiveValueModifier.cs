public class BattleUnitPassiveValueModifier
{
    BaseValueModifier baseValueModifier;
    public BattleUnit source;
    public BattleUnitPassiveValueModifier() { }
    public BattleUnitPassiveValueModifier(BattleUnit source) { this.source = source; }
    public int CalculateModifiedResult(int baseValue)
    {
        return baseValueModifier.CalculateModifiedResult(baseValue);
    }
}

