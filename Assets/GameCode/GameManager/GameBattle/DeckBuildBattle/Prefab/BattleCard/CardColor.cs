using System;
using UnityEngine;

[Serializable]
public class CardColor
{
    public Sprite none;
    public Sprite skill;
    public Sprite attack;
    public Sprite ability;
    public Sprite GetResult(TypeEnum type)
    {
        switch (type)
        {
            case TypeEnum.Skill:
                return skill;
            case TypeEnum.Ability:
                return ability;
            case TypeEnum.Attack:
                return attack;
        }
        return none;
    }
}

