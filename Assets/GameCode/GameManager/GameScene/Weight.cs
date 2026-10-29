using ProjectLumiyaDB;
using System.Collections.Generic;
using UnityEngine;
public static class Weight
{
    public static LevelInfo Pick(this List<LevelInfo> pool)
    {
        List<float> list = new();
        float calculate = 0;
        foreach (var level in pool)
        {
            calculate += level.Weight;
            list.Add(calculate);
        }

        GameDebug.Log(calculate);
        float random = Random.Range(0, calculate);
        GameDebug.Log(random);

        int left = 0;
        int right = list.Count - 1;

        while (left < right)
        {
            int mid = (left + right) / 2;
            if (list[mid] < random)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }
        return pool[left];
    }
}