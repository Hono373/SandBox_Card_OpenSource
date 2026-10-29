using System.Collections.Generic;

public class SelectCamp : ISelectCondition
{
    public List<CampEnum> campEnums = new();
    public bool Check(IBattleUnit unit)
    {
        if (campEnums.Contains(((BattleUnit)unit).camp.camp))
            return true;
        return false;
    }
}
