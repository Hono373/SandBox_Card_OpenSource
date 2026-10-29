using System;

[Serializable]
public class ModifiderCounter
{
    public string type;
    public string key;
    public int count;
    public static ModifiderCounter Create<T>(string key, int count) => new() { type = typeof(T).Name, key = key, count = count };
}
