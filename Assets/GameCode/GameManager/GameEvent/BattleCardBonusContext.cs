public class BattleCardBonusContext
{
    public BattleCard battleCard;
    public BonusContext bonusContext;
    BattleCardBonusContext() { }
    public BattleCardBonusContext(BattleCard battleCard, int bonusBase)
    {
        this.battleCard = battleCard;
        bonusContext = new BonusContext(bonusBase);
    }
    public int Result() => bonusContext.Result();
}
