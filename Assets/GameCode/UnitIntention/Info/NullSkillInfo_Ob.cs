using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = nameof(NullSkillInfo_Ob))]
public class NullSkillInfo_Ob : ScriptableObject, ISkillInfo_Ob
{
    NullSkillInfo_Ob() { }
    public string desc;
    public static NullSkillInfo_Ob Get() => GameAsset.GetSingletonSo<NullSkillInfo_Ob>();
    [SerializeField] SelectorBox_Ob selectorBox;
    public SelectorBox_Ob SelectMode() => selectorBox;
    [SerializeReference] List<ISkillEffectInfo_Ob> effectInfos = new();
    public List<ISkillEffectInfo_Ob> EffectInfos() => effectInfos;
    public List<IntentionUIData_Ob> GetUIData()
    {
        return new List<IntentionUIData_Ob>() { new IntentionUIData_Ob(EffectIconEnum.Unknown, null, null) };
    }
    public string Desc(BattleUnit source, BattleUnit target)
    {
        return desc;
    }
}
