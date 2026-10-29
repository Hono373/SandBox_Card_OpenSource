using DG.Tweening;
using UnityEngine;

public class JumpAnim : UIAnim
{
    public float offsetX;
    public float jumpPower = 1f;
    public int jumpNumb = 1;
    public float duration = 1f;
    public Ease ease = Ease.OutQuad;
    public override Tween Tween(RectTransform rectT, CanvasGroup cg)
    {
        var target = new Vector2(rectT.anchoredPosition.x + offsetX, rectT.anchoredPosition.y);
        return rectT.DOJumpAnchorPos(
            target,
            jumpPower,
            jumpNumb,
            duration
            ).
            SetEase(ease);
    }
    public Tween DurationTween(RectTransform rectT, CanvasGroup cg, float duration)
    {
        var target = new Vector2(rectT.anchoredPosition.x + offsetX, rectT.anchoredPosition.y);
        return rectT.DOJumpAnchorPos(
            target,
            jumpPower,
            jumpNumb,
            duration
            ).
            SetEase(ease);
    }
}