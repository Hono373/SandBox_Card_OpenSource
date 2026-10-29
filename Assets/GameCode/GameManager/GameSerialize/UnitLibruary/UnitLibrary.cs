using System.Collections.Generic;
public class UnitLibrary
{
    /// <summary>
    /// Dict.SetID: Unit.SetID
    /// </summary>
    public Dictionary<string, Unit> unitDict = new();
    UnitLibrary() { }
    public static UnitLibrary Create(GameInfo gameInfo)
    {
        var instance = new UnitLibrary();
        instance.CreateUnit(gameInfo.playerKey, false);
        return instance;
    }
    public Unit CreateUnit(string key, bool temporary)
    {
        string id;
        if (temporary)
        {
            id = unitDict.Keys.RandomID();
        }
        else
        {
            if (unitDict.ContainsKey(key))
            {
                GameDebug.Log("Api误用：正常流程下，CreateUnit不应该创建已经存在的非临时对象");
                return unitDict[key];
            }
            id = key;
        }
        var unit = new Unit(key, id, temporary);
        unitDict[unit.id] = unit;
        return unit;
    }
    public bool ContainsId(string key)
    {
        return unitDict.ContainsKey(key);
    }
    public Unit GetUnitById(string id)
    {
        if (unitDict.TryGetValue(id, out var unit))
        {
            return unit;
        }
        else
        {
            GameDebug.Log("UnitLibruary Get Failed : " + id);
            unitDict[id] = unit = new Unit(id, id, true);
        }
        return unit;
    }
}