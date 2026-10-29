using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用于Choice阶段的敌人可指向性检查
/// </summary>
[Serializable]
public class ChoiceRule
{
    public RangeEnum range;
    [SerializeReference] public List<RangeCondition> conditions = new();
    public ChoiceRule Create()
    {
        var instance = new ChoiceRule();
        instance.range = range;
        foreach (var condition in conditions)
        {
            instance.conditions.Add(condition.Create());
        }
        return instance;
    }
    public void ValidateData()
    {
        if (conditions == null)
        {
            Debug.LogError($"范围 conditions is null");
            return;
        }
    }
    public bool IsTrueChoice(BattleUnit caster, BattleUnit choice)
    {
        var pool = GetTargetPool(caster);
        foreach (var condition in conditions)
        {
            pool = condition.Done(pool);
        }
        if (pool.Contains(choice))
        {
            return true;
        }
        return false;
    }
    List<BattleUnit> GetTargetPool(BattleUnit caster)
    {
        var list = new List<BattleUnit>();
        switch (range)
        {
            case RangeEnum.All:
                list.AddRange(DeckBuildBattle.Active.data.unitDict.GetAll().Values);
                break;
            case RangeEnum.Rival:
                if (caster.camp.camp == CampEnum.Mate)
                    list.AddRange(DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Enemy));
                else if (caster.camp.camp == CampEnum.Enemy)
                    list.AddRange(DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Mate));
                break;
            case RangeEnum.Mate:
                if (caster.camp.camp == CampEnum.Mate)
                    list.AddRange(DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Mate));
                else if (caster.camp.camp == CampEnum.Enemy)
                    list.AddRange(DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Enemy));
                break;
            case RangeEnum.Self:
                list.Add(caster);
                break;
        }
        return list;
    }
}
