using System;

[Serializable]
public class Frame
{
    public Rarity none = new();
    public AttackRarity attack = new();
    public AbilityRarity ability = new();
    public SkillRarity skill = new();
    public Rarity GetResult(TypeEnum type)
    {
        switch (type)
        {
            case TypeEnum.Attack:
                return attack;
            case TypeEnum.Ability:
                return ability;
            case TypeEnum.Skill:
                return skill;
        }
        return none;
    }
}

