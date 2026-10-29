
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueInfo : BaseInfo
{
    [SerializeReference] public List<IDialogueCondition> conditions = new();
    [SerializeReference] public List<IAnim> anims = new();
    public TextInfo speaker = new();
    public TextInfo text = new();

    public DialogueInfo Init(StoryInfo info, Vector2 v)
    {
        Set(info, v);
        return this;
    }
}