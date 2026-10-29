using System;
using System.Collections.Generic;
/// <summary>
/// 加权随机选择器 - 权重越高，被选中的概率越大
/// 原理：权重铺成数轴，随机数落在哪个区间就选哪个
/// </summary>
public class WeightedRandomSelector
{
    List<int> prefixSums;
    /// <summary>
    /// 前缀和数组，eg. 权重 [10, 30, 20] -> 前缀和 [10, 40, 60]
    /// </summary>
    int totalWeight;
    static Random random = new();
    public WeightedRandomSelector(IReadOnlyList<int> weights)
    {
        if (weights.Count == 0)
            throw new ArgumentException("权重列表不能为空");
        prefixSums = new List<int>(weights.Count);
        foreach (var w in weights)
        {
            if (w < 0)
                throw new ArgumentException($"权重不能为负数，索引 {prefixSums.Count} = {w}");
            totalWeight += w;
            prefixSums.Add(totalWeight);
        }
        if (totalWeight <= 0)
            throw new ArgumentException("总权重大于0");
    }
    public int TotalWeight => totalWeight;
    /// <param name="randomValue">范围 [0, TotalWeight) 内的随机整数</param>
    /// <example>权重 [70, 25, 4, 1]，总权重 100 -> randomValue=0~69返回0索引，70~94返回1索引，依此类推</example>
    public int SelectIndex(int randomValue)
    {
        var index = prefixSums.BinarySearch(randomValue);
        return index < 0 ? ~index : index + 1;
    }
    public int SelectIndex()
    {
        var randomValue = random.Next(totalWeight);
        // 二分查找第一个前缀和 > randomValue 的位置
        var index = prefixSums.BinarySearch(randomValue);
        return index < 0 ? ~index : index + 1;
    }
}
