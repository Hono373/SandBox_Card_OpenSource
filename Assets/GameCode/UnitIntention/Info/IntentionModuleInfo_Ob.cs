using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
[Serializable]
public class IntentionModuleInfo_Ob : GameNode_Ob, IIntentionModuleInfo_Ob
{
    [SerializeField] ConditionBox conditionBox = new();
    [SerializeReference][TypeFilter(nameof(GetChildTypes))] List<IGameNode_Ob> childs = new();
    public override IReadOnlyList<IGameNode_Ob> Childs => childs;
    public override ConditionBox ConditionBox => conditionBox;
    public override int Weight => 0;
    static IEnumerable<Type> GetChildTypes()
    {
#if UNITY_EDITOR
        var allTypes = TypeCache.GetTypesDerivedFrom<IGameNode_Ob>();
        return allTypes
        .Where(t => t != typeof(IntentionModuleInfo_Ob) && !t.IsInterface && !t.IsAbstract);
#else
        return new List<Type>();
#endif
    }
}
