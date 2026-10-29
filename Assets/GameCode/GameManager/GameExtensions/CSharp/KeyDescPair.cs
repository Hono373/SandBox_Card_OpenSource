using System;
using Sirenix.OdinInspector;

[Serializable]
public struct KeyDescPair
{
    [ReadOnly] public string key;
    [ReadOnly] public string desc;
    public KeyDescPair(string key, string name)
    {
        this.key = key;
        this.desc = name;
    }
    public override string ToString()
    {
        return $"[{key}]{desc}";
    }
}
