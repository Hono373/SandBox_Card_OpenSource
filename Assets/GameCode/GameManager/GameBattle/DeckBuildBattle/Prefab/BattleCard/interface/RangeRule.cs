using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RangeRule
{
    public bool active = false;
    public RangeEnum baseRange;
    /// <summary>
    /// 2026-10-23 候选池是否排除施法者自己。
    /// Mate（友军）池取的是整个同阵营，本身包含施法者，所以「只给队友加盾、永远不给自己」
    /// 这类技能必须显式打开这一位；否则随机时会打到自身上。
    /// 独自一人时池子会被清空，AIChoice 会回退到 caster —— 那是另一条路，由意图条件先拦住。
    /// </summary>
    public bool excludeSelf = false;
    [SerializeReference] public List<RangeCondition> conditions = new();
    public RangeRule Create()
    {
        var instance = new RangeRule();
        instance.active = active;
        instance.baseRange = baseRange;
        instance.excludeSelf = excludeSelf;
        foreach (var condition in conditions)
        {
            instance.conditions.Add(condition.Create());
        }
        return instance;
    }
    internal void ValidateData()
    {
        if (conditions == null)
        {
            Debug.LogError($"范围 conditions 为空");
            return;
        }
        foreach (var condition in conditions)
        {
            if (condition == null || condition.GetType().IsAbstract)
                Debug.LogError($"范围存在无效 condition（null 或抽象类）");
            else
                condition.ValidateData();
        }
    }
    public List<BattleUnit> GetTargetPool(BattleUnit caster)
    {
        var list = new List<BattleUnit>();
        switch (baseRange)
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
        // 2026-10-23 先剔自己再剔死人：顺序不影响结果，但先剔自己能让下面的空池语义更干净
        if (excludeSelf)
            list.Remove(caster);
        list.RemoveAll(u => u.current.isDead || u.current.nearDeath || u.data.hp <= 0);
        return list;
    }
    public List<BattleUnit> GetTargetList(BattleUnit caster, BattleUnit choice)
    {
        var result = new List<BattleUnit> { choice };
        if (!active) return result;

        var list = GetTargetPool(caster);
        list.Remove(choice);

        foreach (var condition in conditions)
        {
            list = condition.Done(list);
        }

        result.AddRange(list);
        return result;
    }
    public List<BattleUnit> GetTargetList(BattleUnit caster)
    {
        var list = GetTargetPool(caster);

        foreach (var Filter in conditions)
        {
            list = Filter.Done(list);
        }
        return list;
    }
    public List<BattleUnit> GetTargetList(BattleUnit caster, List<BattleUnit> targetPool)
    {
        if (!active)
        {
            targetPool.RemoveAll(u => u.current.isDead || u.current.nearDeath || u.data.hp <= 0);
            return targetPool;
        }
        return GetTargetList(caster);
    }
    public BattleUnit AIChoice(BattleUnit caster)
    {
        var list = GetTargetList(caster);
        foreach (var condition in conditions)
        {
            list = condition.Done(list);
        }

        if (list.Random(out var result))
        {
            return result;
        }
        return caster;
    }

}