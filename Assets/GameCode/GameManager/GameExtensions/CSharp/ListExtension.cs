using System;
using System.Collections;
using System.Collections.Generic;

public static class ListExtension
{
    public static T FirstOfType<T>(this IList list) where T : class
    {
        foreach (var item in list)
            if (item is T result) return result;
        return null;
    }

    public static object FirstOfType(this IList list, Type type)
    {
        foreach (var item in list)
            if (item?.GetType() == type)
                return item;
        return null;
    }

    public static bool Random<T>(this List<T> list, out T randomEle)
    {
        if (list == null || list.Count == 0)
        {
            randomEle = default;
            return false;
        }
        randomEle = list[UnityEngine.Random.Range(0, list.Count)];
        return true;
    }

    public static List<T> GetRandom<T>(this List<T> list, int count)
    {
        var result = new List<T>(count);
        if (list == null || list.Count == 0 || count <= 0)
            return result;

        count = count > list.Count ? list.Count : count;

        var indices = new HashSet<int>();
        while (indices.Count < count)
        {
            indices.Add(UnityEngine.Random.Range(0, list.Count));
        }

        foreach (var idx in indices)
        {
            result.Add(list[idx]);
        }

        return result;
    }
}
