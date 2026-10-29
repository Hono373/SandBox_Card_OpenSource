using System;
using System.Collections.Generic;

[Serializable]
public class ModifierData
{
    public string id = string.Empty;
    public string key = string.Empty;
    public int layer;
    public bool only;
    public Dictionary<string, int> paramsDict = new();
    public string owner;
}
