using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class IntentionModule_Ob
{
    [JsonProperty][SerializeReference] IntentionData_Ob intentionData;
    [SerializeField] IntentionModuleUIListener_Ob uiModule = new();
    public IntentionModule_Ob() { }

#nullable enable
    //根据单位状态走cfg树判定，走到叶节点即为本次意图技能
    public ISkillInfo_Ob? GetSKill(BattleUnit unit, IIntentionModuleInfo_Ob info)
    {
        var path = GetPath(unit, info);
        intentionData = new IntentionData_Ob(path);
        return GetSkillFromPath(path, info);
    }
    //遍历cfg树，记录路径索引
    List<int?> GetPath(BattleUnit unit, IIntentionModuleInfo_Ob info)
    {
        var path = new List<int?>();
        IGameNode_Ob node = info;

        while (!node.IsEnd)
        {
            var index = node.GetChildIndex(unit);
            path.Add(index);
            if (!index.HasValue) break;
            node = node.Childs[index.Value];
        }

        return path;
    }
    //按存档路径重走一遍，拿到叶节点SkillInfo
    SkillInfo_Ob? GetSkillFromPath(List<int?> path, IIntentionModuleInfo_Ob info)
    {
        IGameNode_Ob node = info;

        foreach (var idx in path)
        {
            if (!idx.HasValue) return null;
            node = node.Childs[idx.Value];
        }

        return node is SkillInfo_Ob result ? result : null;
    }
#nullable disable
}
