using System;

[Serializable]
public class Background
{
    public CardColor none;
    public CardColor red;
    public CardColor GetResult(ColorEnum color)
    {
        switch (color)
        {
            case ColorEnum.None:
                return none;
            case ColorEnum.Red:
                return red;
        }
        return none;
    }
}

