using System;
using UnityEngine;

[Serializable]
public class EndInfo : BaseInfo
{
    public EndInfo Init(StoryInfo info, Vector2 v)
    {
        Set(info, v);
        return this;
    }
}
