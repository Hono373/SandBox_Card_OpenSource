
using System.Collections.Generic;
using UnityEngine;
public class AnimInfo : BaseInfo
{
    [SerializeReference] public List<IAnim> anims = new();
    public AnimInfo Init(StoryInfo info, Vector2 v)
    {
        Set(info, v);
        return this;
    }
}