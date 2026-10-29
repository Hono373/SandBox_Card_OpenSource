using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 无限滚动列表（数据驱动全量刷新版）
/// 固定行池，滚动时按 startIndex 整批重钉行位置与数据。
/// 外部只需 Init(工厂) + Fresh(list)，滚动/复用/越界处理全部内部封装。
/// </summary>
public class InfiniteList : MonoBehaviour
{
    public int countInRow = 5;
    /// <summary>最小行池数量（Init 时会按视口自动扩容到 可见行数 + 2）</summary>
    public int countInColumn = 5;

    public float paddingBotton = 200;
    /// <summary>顶部冗余：首行卡牌顶部到视口顶的间距，容纳超框元素（费用等）</summary>
    public float paddingTop = 80;

    public float midY = 720;
    public float width = 2000;
    public float spacingY = 100;

    public RectTransform scroll;
    public RectMask2D rectMask2D;

    public List<RowContent> pool = new();
    public List<IInfiniteData> sourceData = new();

    Func<Transform, IInfiniteObject> createFunc;
    Rect objectRect;
    float rowSpacing;
    int rowCount;
    public float moveLimit;

    /// <summary>屏幕顶部的可视边界（世界 y）</summary>
    float Top => Screen.height / 2 - rectMask2D.padding.w - paddingTop;

    public void Init(Func<Transform, IInfiniteObject> func)
    {
        this.GetComponentSafety(out rectMask2D);
        createFunc = func;

        // 先建最小池，量出单行 Rect 与行距
        EnsurePoolRows(countInColumn);
        objectRect = pool[0].PickRect;
        rowSpacing = objectRect.height + spacingY;

        // 按视口扩容：可见行数 + 2 缓冲，全屏 2560x1440 也自动覆盖
        var visibleRows = Mathf.CeilToInt((Screen.height - rectMask2D.padding.w) / rowSpacing);
        EnsurePoolRows(visibleRows + 2);
    }

    void EnsurePoolRows(int count)
    {
        for (var i = pool.Count; i < count; i++)
        {
            var content = RowContent.Create(createFunc, countInRow, 0, scroll);
            content.LayoutInside(width, countInRow);
            pool.Add(content);
        }
    }

    internal void Fresh(List<IInfiniteData> data)
    {
        sourceData = data;
        rowCount = Mathf.CeilToInt((float)sourceData.Count / countInRow);
        moveLimit = CalculateLimit();
        scroll.anchoredPosition = Vector2.zero;
        Refresh();
    }

    float CalculateLimit()
    {
        return rowCount * rowSpacing - Screen.height + rectMask2D.padding.w - spacingY + paddingBotton;
    }

    internal void OnDrag(Vector2 vector2)
    {
        if (moveLimit <= 0) return;
        var target = scroll.anchoredPosition + vector2;
        scroll.anchoredPosition = new Vector2(0, Mathf.Clamp(target.y, 0, moveLimit));
        Refresh();
    }

    /// <summary>
    /// 根据滚动量重算 startIndex，整批重钉池中所有行的位置与数据。
    /// 行坐标是内容坐标（不随滚动量变），滚动位移由 scroll 承担，视觉连续。
    /// </summary>
    void Refresh()
    {
        var startIndex = Mathf.FloorToInt(scroll.anchoredPosition.y / rowSpacing);
        var n = 0;
        foreach (var content in pool)
        {
            var rowIndex = startIndex + n;
            content.index = rowIndex;
            content.recttransform.anchoredPosition = RowPosition(rowIndex);

            var valid = rowIndex >= 0 && rowIndex < rowCount;
            content.SetActive(valid);
            if (valid)
                content.Fresh(sourceData);

            n++;
        }
    }

    /// <summary>第 rowIndex 行的内容坐标（相对 scroll，屏幕中心锚点）</summary>
    Vector2 RowPosition(int rowIndex)
    {
        return new Vector2(0, Top - objectRect.height / 2 - rowSpacing * rowIndex);
    }
}
