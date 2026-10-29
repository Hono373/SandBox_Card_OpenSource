using System;
using System.Collections.Generic;
using UnityEngine;

public class RowContent : MonoBehaviour
{
    public RectTransform recttransform;
    public int index;
    public int count;
    public IInfiniteObject[] objectArray;

    internal static RowContent Create
        (Func<Transform, IInfiniteObject> func, int count, int index, Transform scroll)
    {
        var content = GameAsset.Instantiate<RowContent>(scroll);

        content.GetComponentSafety(out content.recttransform);
        content.index = index;
        content.count = count;
        content.objectArray = new IInfiniteObject[count];

        for (var i = 0; i < count; i++)
        {
            content.objectArray[i] = func.Invoke(content.transform);
        }

        return content;
    }

    internal void Fresh(List<IInfiniteData> list)
    {
        var i = 0;
        foreach (var ele in objectArray)
        {
            var target = i + index * count;
            if (target >= list.Count)
            {
                ele.SetActive(false);
            }
            else
            {
                ele.SetActive(true);
                ele.Fresh(list[target]);
            }
            i++;
        }
    }

    internal void SetActive(bool v)
    {
        foreach (var ele in objectArray)
        {
            ele.SetActive(v);
        }
    }

    Vector2[] CalcluateLayout(float width, int count)
    {
        var posArray = new Vector2[count];
        var first = new Vector2(-width / 2, 0);

        for (int i = 0; i < count; i++)
        {
            posArray[i] = first + CalculateOffsetSafety(count, width, i);
        }

        return posArray;

        Vector2 CalculateOffsetSafety(int count, float width, int i)
        {
            if (count > 1)
            {
                return new Vector2(width / (count - 1), 0) * i;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    public void LayoutInside(float width, int count)
    {
        var posArray = CalcluateLayout(width, count);

        for (var i = 0; i < objectArray.Length; i++)
        {
            objectArray[i].SetPos(posArray[i]);
        }
    }

    public Rect PickRect
    {
        get => objectArray[0].Rect;
    }
}