public static class BattleUnitDataQuery
{
    public static Unit GetUnit(this BattleUnit battleUnit)
    {
        return GameSerialize.Active.unitLibrary.GetUnitById(battleUnit.data.id);
    }
}
