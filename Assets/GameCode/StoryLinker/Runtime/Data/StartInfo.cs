using System;
using UnityEngine;

[Serializable]
public class StartInfo : BaseInfo
{
    public string title = string.Empty;
    [TextArea(10, 40)] public string describe = string.Empty;
    [HideInInspector] public Sprite background;
    [HideInInspector] public AudioClip bgm;
    public StartInfo Init(StoryInfo info, Vector2 v)
    {
        Set(info, v);
        return this;
    }
}

