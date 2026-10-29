public class BattleUnitBonusContext
{
    public BattleUnit battleUnit;
    public BonusContext bonusContext;
    BattleUnitBonusContext() { }
    public BattleUnitBonusContext(BattleUnit battleUnit, int bonusBase)
    {
        this.battleUnit = battleUnit;
        bonusContext = new BonusContext(bonusBase);
    }
    public int Result() => bonusContext.Result();
}
