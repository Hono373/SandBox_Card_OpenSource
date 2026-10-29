using Newtonsoft.Json;
using ProjectLumiyaDB;
using System.Collections.Generic;

public class LevelData
{
    public string key;
    public bool isOver;
    public List<LevelData> Linked;
    public void Set(string key)
    {
        this.key = key;
    }
    [JsonIgnore] public LevelInfo Info =>null;
}
