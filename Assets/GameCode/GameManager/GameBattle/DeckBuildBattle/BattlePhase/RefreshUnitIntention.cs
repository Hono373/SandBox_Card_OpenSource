public class RefreshUnitIntention
{
    public BattleUnit unit;
    public RefreshUnitIntention Start(BattleUnit unit)
    {
        this.unit = unit;
        return this;
    }
}
