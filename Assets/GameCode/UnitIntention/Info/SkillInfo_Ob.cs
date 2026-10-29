using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能
/// 决定技能的实现
/// </summary>
[Serializable]
public class SkillInfo_Ob : GameNode_Ob, ISkillInfo_Ob
{
    [SerializeField] SelectorBox_Ob selectorBox = new();
    public SelectorBox_Ob SelectMode() => selectorBox;
    [SerializeReference] List<ISkillEffectInfo_Ob> skillEffectInfo = new();
    public List<ISkillEffectInfo_Ob> EffectInfos() => skillEffectInfo;

    #region 抽象
    public override bool IsEnd => true;
    public override ConditionBox ConditionBox => null;
    public override IReadOnlyList<IGameNode_Ob> Childs => null;
    public override IReadOnlyList<int> ChildsWeights() => null;
    [SerializeField][MinValue(1)] int weight = 1;
    public override int Weight => weight;
    #endregion

    public string Desc(BattleUnit source, BattleUnit target)
    {
        var result = new List<string>();
        foreach (var effect in skillEffectInfo)
        {
            result.Add(effect.Desc(source, target));
        }
        return string.Join("\n", result);
    }
    public List<IntentionUIData_Ob> GetUIData()
    {
        var result = new List<IntentionUIData_Ob>();
        foreach (var effect in skillEffectInfo)
        {
            result.Add(effect.IntentionsData());
        }
        return result;
    }

}
