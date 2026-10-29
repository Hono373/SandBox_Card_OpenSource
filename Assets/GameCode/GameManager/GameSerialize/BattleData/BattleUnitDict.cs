using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleUnitDict
{
    [SerializeReference][UnityEngine.SerializeField][JsonProperty] Dictionary<string, BattleUnit> totalDict = new();
    [SerializeReference] [UnityEngine.SerializeField][JsonProperty] Dictionary<CampEnum, List<string>> campDict = new();
    public BattleUnitDict()
    {
        campDict.Add(CampEnum.Enemy, new());
        campDict.Add(CampEnum.Mate, new());
    }
    public List<BattleUnit> this[CampEnum team]
    {
        get
        {
            var list = new List<BattleUnit>();
            foreach (var item in campDict[team])
            {
                list.Add(totalDict[item]);
            }
            return list;
        }
    }
    public void Add(CampEnum team, BattleUnit unit)
    {
        totalDict[unit.data.id] = unit;
        campDict[team].Add(unit.data.id);
    }
    public BattleUnit GetBattleUnitById(string id)
    {
        if (totalDict.TryGetValue(id, out var unit))
        {
            return unit;
        }
        GameDebug.Log("GetBattleUnitById faild");
        return null;
    }
    public List<BattleUnit> GetByCamp(CampEnum camp)
    {
        var list = new List<BattleUnit>();
        if (campDict.TryGetValue(camp, out var unitList))
        {
            foreach (var id in unitList)
            {
                list.Add(totalDict[id]);
            }
        }
        return list;
    }
    public Dictionary<string, BattleUnit> GetAll() => this.totalDict;
    public HashSet<string> GetHash(CampEnum team)
    {
        HashSet<string> hash = new();
        if (team == CampEnum.All)
        {
            hash.UnionWith(campDict[CampEnum.Mate]);
            hash.UnionWith(campDict[CampEnum.Mate]);
            hash.UnionWith(campDict[CampEnum.Enemy]);
            return hash;
        }
        hash.UnionWith(campDict[team]);
        return hash;
    }

    internal void Remove(string id)
    {
        var unit = totalDict[id];
        campDict[unit.camp.camp].Remove(id);
        totalDict.Remove(id);
    }
}
