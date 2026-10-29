using DG.Tweening;
using UnityEngine;
public class UILeaveAnim : UIAnim
{
    [SerializeField] float duration = 0.75f;
    [SerializeField] Ease ease = Ease.Linear;
    public override Tween Tween(RectTransform rectT, CanvasGroup cg)
    {
        return cg.
            DOFade(0, duration).
            SetEase(ease);
    }
}
