public class RivalSelectCheck : ISelectCheck
{
    public bool IsTrueSelect(BattleUnit source, BattleUnit target)
    {
        if (target.camp.camp != source.camp.camp)
            return true;
        return false;
    }
}
