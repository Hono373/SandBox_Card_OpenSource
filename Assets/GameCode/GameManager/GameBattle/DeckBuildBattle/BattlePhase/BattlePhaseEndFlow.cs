public class BattlePhaseEndFlow : RootBattleFlow
{
    public BattlePhaseEnd battlePhaseEnd;
    public BattlePhaseEndFlow Start(BattlePhaseEnd battlePhaseEnd)
    {
        this.battlePhaseEnd = battlePhaseEnd;
        Init();
        return this;
    }
}
