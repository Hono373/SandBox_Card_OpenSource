using System;
using UnityEngine;

[Serializable]
public class BattleCardGraphic
{
    public ColorEnum color;
    public TypeEnum type;
    public RarityEnum rarity;
    public Sprite BannerResult() => BattleCardUI.Info().banner.Get(rarity);
    public Sprite CardBgResult() => BattleCardUI.Info().bg.GetResult(color).GetResult(type);
    public Sprite FrameResult() => BattleCardUI.Info().frame.GetResult(type).Get(rarity);
    public string TypeResult() => type.ToString();

    public BattleCardGraphic Create() => new() { color = color, type = type, rarity = rarity };
}
