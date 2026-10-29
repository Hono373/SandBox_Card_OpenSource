using System;
using UnityEngine;

public static class RectTransformExtensions
{
    public static float IsLeftAnchor(this RectTransform rect, RectTransform parent)
    {
        if (rect.anchorMax.x == 0 && rect.anchorMin.x == 0)
        {
            var pos = rect.anchoredPosition;
            return pos.x - parent.rect.width / 2;
        }
        else { throw new Exception(); }
    }
    public static RectTransform GetRectTransform(this MonoBehaviour target)
    {
        if (!target.TryGetComponent(out RectTransform canvasGroup))
        {
            canvasGroup = target.gameObject.AddComponent<RectTransform>();
        }
        return canvasGroup;
    }
}
