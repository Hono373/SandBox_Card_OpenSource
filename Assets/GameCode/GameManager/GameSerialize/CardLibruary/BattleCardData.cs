using System;
using System.Collections.Generic;
[Serializable]
public class BattleCardData : IInfiniteData
{
    public bool temporary;
    public string id;
    public string key;
    internal string owner;
    public Dictionary<string, int> dict = new();
    internal static BattleCardData Create(string key, string id, string playerKey, bool temporary)
    {
        var instance = new BattleCardData() { key = key, id = id, owner = playerKey, temporary = temporary };
        return instance;
    }
}