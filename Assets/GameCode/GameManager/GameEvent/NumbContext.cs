public class NumbContext : BattleCardCalculateContext
{
    public NumbContext(BattleCard card)
    {
        Card = card;
        // ValueDefault = Card.IRange().BaseNumb;
    }
}
