public class ActiveDmgPhase
{
    public BattleUnit source;
    public BattleCard card;
    public BaseValueModifier valueModifier = new BaseValueModifier();
    public ActiveDmgPhase(BattleUnit source, BattleCard battleCard)
    {
        this.source = source;
        card = battleCard;
    }
    public int CalculateModifiedResult(int baseValue)
    {
        return valueModifier.CalculateModifiedResult(baseValue);
    }
}
