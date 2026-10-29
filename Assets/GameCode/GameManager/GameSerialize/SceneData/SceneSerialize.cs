using Newtonsoft.Json;
using ProjectLumiyaDB;
using System.Collections.Generic;
using UnityEngine;
public class SceneSerialize
{
    public MapData mapData = null;
    public MapData extend = null;
    public int ending;
    [JsonIgnore] public List<LevelInfo> levelPool = new();
    [JsonIgnore] GameManager game;
    public void Init(GameManager game) => this.game = game;
    public void Set(TaskData data)
    {
        if (data.Info == null) throw new("data.Info == null");
        var info = data.Info;
        CreatePool();
        mapData = new MapData();
        mapData.Set(info, levelPool);
    }
    private void CreatePool()
    {
        // foreach (var info in LubanExtensions.LevelDB.DataList)
        // {
        //     if (IsDedicated(info)) continue;
        //     if (!IsSubsetFlag(info)) continue;
        //     levelPool.Add(info);
        // }
        GameDebug.Log($"levelPool.Count:{levelPool.Count}");
    }
    bool IsDedicated(LevelInfo info)
    {
        return info.Dedicated;
    }
    bool IsSubsetFlag(LevelInfo info)
    {
        return false;/*originalInfo.Flag.IsSubsetOf(SerializeManager.GetActivity().flag)*/;
    }
}
