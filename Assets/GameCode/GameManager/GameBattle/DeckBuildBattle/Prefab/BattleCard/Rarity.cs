using System;
using UnityEngine;

[Serializable]
public class Rarity
{
    public Sprite none;
    public Sprite common;
    public Sprite uncommon;
    public Sprite rare;
    public Sprite Get(RarityEnum rarity)
    {
        switch (rarity)
        {
            case RarityEnum.Common:
                return common;
            case RarityEnum.Uncommon:
                return uncommon;
            case RarityEnum.Rare:
                return rare;
        }
        return none;
    }
}

