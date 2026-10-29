using DG.Tweening;
using UnityEngine;
public class UIScaleAnim : UIAnim
{
    [SerializeField] float scaleIncrease = 0.2f;
    [SerializeField] float duration = 0.75f;
    [SerializeField] Ease ease = Ease.Linear;
    public override Tween Tween(RectTransform rectT, CanvasGroup cg)
    {
        return rectT.
            DOScale(1 + scaleIncrease, duration).
            SetEase(ease);
    }
    public Tween Tween(RectTransform rectT, CanvasGroup cg, float scaleIncrease)
    {
        return rectT.
            DOScale(1 + scaleIncrease, duration).
            SetEase(ease);
    }
}
