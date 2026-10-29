using System;
using System.Collections.Generic;

[Serializable]
public class OutPort
{
    public string guid = string.Empty;
    public List<string> nextGuids = new();
}
