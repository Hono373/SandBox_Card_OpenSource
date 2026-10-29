using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class UnitIntention_SkillGroup : UnitIntention
{
    [SerializeReference] public List<UnitIntention> intentions = new();
    public override UnitIntention Create() => Create(this);
    public static UnitIntention_SkillGroup Create(UnitIntention_SkillGroup info)
    {
        var result = new UnitIntention_SkillGroup();
        info.CopyBaseTo(result);
        foreach (var intention in info.intentions)
        {
            result.intentions.Add(intention.Create());
        }
        return result;
    }
    public override BattleUnitSkill GetResult(BattleUnit unit)
    {
        return GetResult(ConditionFliter(unit), unit);
    }
    public BattleUnitSkill GetResult(List<UnitIntention> childs, BattleUnit unit)
    {
        if (childs == null || childs.Count == 0)
            return null;

        var weights = new List<int>(childs.Count);
        foreach (var child in childs)
        {
            weights.Add(child.weight);
        }

        var selector = new WeightedRandomSelector(weights);
        var index = selector.SelectIndex();
        var result = childs[index].GetResult(unit);
        GameDebug.Log($"{nameof(UnitIntention_SkillGroup)}: {unit.data.key} 候选数={childs.Count} 权重=[{string.Join(",", weights)}] 选中index={index} 结果={result.GetType().Name}(effectType={result.effectTypeEnum})");
        return result;
    }
    List<UnitIntention> ConditionFliter(BattleUnit unit)
    {
        var fliter = new List<UnitIntention>();
        var history = string.Join(",", unit.skillHistory.ConvertAll(h => h.effectTypeEnum));
        foreach (var intention in intentions)
        {
            var pass = intention.fliter.Result(unit);
            GameDebug.Log($"{nameof(UnitIntention_SkillGroup)}: {unit.data.key} 节点 {intention.GetType().Name} w={intention.weight} 条件数={intention.fliter.Count} 规则=[{string.Join(" | ", intention.fliter.Rules)}] 历史=[{history}] 通过={pass}");
            if (pass)
            {
                fliter.Add(intention);
            }
        }
        return fliter;
    }
    /// <summary>配置自检：打印每个子节点的权重与条件明细。条件数恒为 0 说明资源里的 rules 没被反序列化（字段不可序列化，或 SerializeReference 类型没解析到）。</summary>
    internal override void ValidateData()
    {
        foreach (var intention in intentions)
        {
            var rules = intention.fliter.Rules;
            Debug.Log($"{nameof(UnitIntention_SkillGroup)} 子节点 {intention.GetType().Name} weight={intention.weight} 条件数={rules.Count} 明细=[{string.Join(", ", rules)}]");
            intention.ValidateData();
        }
    }
}
