using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RangeRandom : RangeCondition
{
    public int count;
    public override List<BattleUnit> Done(List<BattleUnit> list)
    {
        // 拷贝一份，避免修改传入的原始列表
        var shuffled = new List<BattleUnit>(list);
        var takeCount = Mathf.Min(count, shuffled.Count);

        // Fisher-Yates 部分洗牌：只打乱前 takeCount 个
        for (int i = 0; i < takeCount; i++)
        {
            var pickIndex = UnityEngine.Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[pickIndex]) = (shuffled[pickIndex], shuffled[i]);
        }

        return shuffled.GetRange(0, takeCount);
    }

    internal override RangeCondition Create()
    {
        var instance = new RangeRandom();
        instance.count = count;
        return instance;
    }
}
