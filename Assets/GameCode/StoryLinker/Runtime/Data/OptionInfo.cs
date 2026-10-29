
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class OptionInfo : BaseInfo
{
    public bool needCondition;
    [SerializeReference] public List<IDialogueCondition> conditions = new();
    public TextInfo text = new();

    public OptionInfo Init(StoryInfo info, Vector2 v)
    {
        Set(info, v);
        return this;
    }
}

