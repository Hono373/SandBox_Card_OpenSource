using System.Collections.Generic;

/// <summary>所有卡的运行时实例集合。</summary>
public class CardLibrary
{
    public Dictionary<string, Card> dict = new();
    CardLibrary() { }

    /// <summary>
    /// 为所有已配置的卡建立运行时实例。
    /// 数据源为 GameDB.cardInfos（由 Luban 从 DB.xlsx 导出、游戏加载的 CardInfo.json），
    /// 不再依赖 GameInfo 里的卡牌清单缓存。
    /// </summary>
    public static CardLibrary Create(GameInfo gameInfo)
    {
        var instance = new CardLibrary();
        foreach (var key in GameDB.Get().cardInfos.Keys)
        {
            var cardState = Card.Create(key);
            instance.dict.Add(key, cardState);
        }
        return instance;
    }
}
