public class PassiveDmgPhase
{
    public BattleUnit source;
    public BattleCard card;
    PassiveDmgPhase() { }
    public BaseValueModifier valueModifier = new BaseValueModifier();
    public PassiveDmgPhase(BattleUnit source, BattleCard battleCard)
    {
        this.source = source;
        card = battleCard;
    }
    public int CalculateModifiedResult(int baseValue)
    {
        return valueModifier.CalculateModifiedResult(baseValue);
    }
}
