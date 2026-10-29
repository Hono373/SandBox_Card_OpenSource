using DG.Tweening;
using System;
using UnityEngine;
[Serializable]
public abstract class UIAnim
{
    /// <summary>
    /// 用于进行打断
    /// </summary>
    public abstract Tween Tween(RectTransform rectT, CanvasGroup cg);
    public Tween Tween(UIGroup uiGroup)
    {
        return Tween(uiGroup.RectT, uiGroup.CG);
    }
}
