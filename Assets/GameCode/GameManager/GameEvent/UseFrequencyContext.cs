public class UseFrequencyContext : BattleCardCalculateContext
{
    public UseFrequencyContext(BattleCard card)
    {
        Card = card;
        ValueDefault = 1;
    }
}
