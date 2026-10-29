public class BattleUnitRefresh
{
    BattleUnit battleUnit;
    BattleUnitRefresh() { }
    public BattleUnitRefresh(BattleUnit unit)
    {
        battleUnit = unit;
        ForceRefresh();
    }
    public BattleUnitRefresh ForceRefresh()
    {
        Reset();
        return this;
    }
    public CampEnum GetScope()
    {
        return battleUnit.camp.camp;
    }
    public void Reset()
    {

    }
}
