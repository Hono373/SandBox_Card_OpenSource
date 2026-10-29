using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseInfo
{
    [HideInInspector] public StoryInfo parent = null;

    public InPort inPort = new();
    public Vector2 pos = Vector2.zero;
    public OutPort outPort = new();

    public void Set(StoryInfo info, Vector2 v)
    {
        parent = info;
        if (this is not StartInfo)
            inPort.guid = parent.NewGuid();
        if (this is not EndInfo)
            outPort.guid = parent.NewGuid();
        pos = v;
        if (this is not StartInfo)
            parent.list.Add(this);
    }

    public void LinkTo(string nextGuid)
    {
        if (!outPort.nextGuids.Contains(nextGuid))
            outPort.nextGuids.Add(nextGuid);
    }

    public void DisConnect(string nextGuid)
    {
        outPort.nextGuids.Remove(nextGuid);
    }

    public void UpdatePos(Vector2 pos)
    {
        this.pos = pos;
    }

    public void Cleaner(BaseInfo removed)
    {
        outPort.nextGuids.Remove(removed.inPort.guid);
    }
}