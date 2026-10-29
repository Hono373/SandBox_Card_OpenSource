using DG.Tweening;
using System;

[Serializable]
public class CardShowAnime
{
    public Sequence Create()
    {
        var seq = GameAnim.AutoPlay(nameof(CardShowAnime));
        return seq;
    }
}

