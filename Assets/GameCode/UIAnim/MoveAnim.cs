using DG.Tweening;
using UnityEngine;

public class MoveAnim : UIAnim
{
    public Vector2 pos;
    public float duration;
    public Ease ease = Ease.Linear;
    public bool reversal;
    public override Tween Tween(RectTransform rectT, CanvasGroup cg)
    {
        if (!reversal)
        {
            return rectT.
            DOAnchorPos(rectT.anchoredPosition + pos, duration).
            SetEase(ease);

        }
        else
        {
            var endPos = rectT.anchoredPosition;
            var startPos = endPos + pos;
            rectT.anchoredPosition = startPos;
            return rectT.
            DOAnchorPos(endPos, duration).
            SetEase(ease);
        }
    }
}
