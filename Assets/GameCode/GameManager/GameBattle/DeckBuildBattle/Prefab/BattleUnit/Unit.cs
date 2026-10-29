using System;

[Serializable]
public class Unit : IManager
{
    public bool temporary;
    public string id;
    public string key;

    public UnitData data;

    public Unit() { }
    public Unit(string key, string id, bool temporary)
    {
        this.temporary = temporary;
        this.id = id;
        this.key = key;
        var info = UnitInfo.Get(key);
        data = new UnitData(info);
    }
    public void Init() { }
}
