using System;
using System.Collections.Generic;
[Serializable]
public class PlayerData
{
    public List<string> cardPile = new();
    public Dictionary<string, BattleCard> cardDict = new();
    PlayerData() { }
    public static PlayerData Create(GameInfo info)
    {
        var instance = new PlayerData();
        instance.InitPlayerDeck(info);
        return instance;
    }
    void InitPlayerDeck(GameInfo info)
    {
        foreach (var createInfo in info.cardDeckInitial)
        {
            var n = createInfo.numb;
            while (n > 0)
            {
                var card = BattleCard.Create(createInfo.Key, cardDict.Keys.RandomID(), info.playerKey, false);
                cardDict.Add(card.data.id, card);
                cardPile.Add(card.data.id);
                n--;
            }
        }
    }
}
