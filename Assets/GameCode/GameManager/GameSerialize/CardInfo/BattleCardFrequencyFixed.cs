using System;

[Serializable]
public class BattleCardFrequencyFixed : IBattleCardFrequency
{
    public virtual int BaseFrequency => 1;
    public int Frequency(BattleCard card)
    {
        if (!frequencyCache.HasValue)
        {
            // UseFrequencyContext.Create 缺失，临时返回 BaseFrequency
            frequencyCache = BaseFrequency;
        }
        return frequencyCache.Value;
    }
    int? frequencyCache;
}

