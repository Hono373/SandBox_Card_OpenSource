﻿using UnityEngine;
public class UIGroup
{
    RectTransform rectT;
    CanvasGroup cg;
    public static UIGroup Create(MonoBehaviour target, out UIGroup uiGroup)
    {
        uiGroup = new UIGroup();
        target.GetComponentSafety(out uiGroup.rectT);
        target.GetComponentSafety(out uiGroup.cg);
        uiGroup.SetActive(true);
        return uiGroup;
    }
    public CanvasGroup CG => cg;
    public RectTransform RectT => rectT;
    public bool GetIsShow()
    {
        return cg.alpha == 1;
    }
    public bool GetBlocksRaycasts()
    {
        return cg.blocksRaycasts;
    }
    public void Ray(bool clickAble)
    {
        cg.blocksRaycasts = clickAble;
    }
    public void Alpha(bool show)
    {
        if (show) cg.alpha = 1;
        else cg.alpha = 0;
    }
    public void SetActive(bool value)
    {
        Ray(value);
        Alpha(value);
    }
}
