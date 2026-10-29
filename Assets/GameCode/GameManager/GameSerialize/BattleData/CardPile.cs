using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
[Serializable]
public class CardPile
{
    public List<BattleCard> list = new();
    public void Create(List<BattleCard> deck)
    {
        foreach (var cardData in deck)
        {
            list.Add(cardData);
        }
    }
    public BattleCard Get(string id)
    {
        foreach (var card in list)
        {
            if (card.data.id == id)
            {
                return card;
            }
        }
        return null;
    }
    public void Add(BattleCard drawnCard)
    {
        list.Add(drawnCard);
    }
    public void AddThese(List<BattleCard> cardList)
    {
        list.AddRange(cardList);
    }
    public void Refill(CardPile deck)
    {
        list.AddRange(deck.list);
    }
    public void Remove(BattleCard data)
    {
        list.Remove(data);
    }
    public void RemoveThese(List<BattleCard> removeList)
    {
        foreach (var cardData in removeList)
        {
            var success = list.Remove(cardData);
            if (!success)
            {
                GameDebug.Log("Remove Card Failed: " + cardData.data.key);
            }
        }
    }
    public void Clear()
    {
        list.Clear();
    }
    public int Count() => list.Count;
    public override string ToString()
    {
        var log = "CardPile\n";
        foreach (var card in list)
        {
            log += card.data.key + "\n";
        }
        return log;
    }
    public bool Contains(BattleCard cardData)
    {
        if (list.Contains(cardData))
        {
            return true;
        }
        return false;
    }
    public BattleCard Index(int v)
    {
        return list[v];
    }
    public void Shuffle()
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
