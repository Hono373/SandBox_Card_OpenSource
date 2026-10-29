using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
[Serializable]
public class DeckBuildBattleDeck
{

    [ShowInInspector] public Dictionary<PileEnum, CardPile> dict = new();
    DeckBuildBattleDeck() { }
    public static DeckBuildBattleDeck Create(List<string> keys, Dictionary<string, BattleCard> collection)
    {
        var instance = new DeckBuildBattleDeck();
        instance.dict.Add(PileEnum.TotalPile, new());
        instance.dict.Add(PileEnum.Draw, new());
        instance.dict.Add(PileEnum.Hand, new());
        instance.dict.Add(PileEnum.Discard, new());
        instance.dict.Add(PileEnum.Removed, new());
        instance.dict.Add(PileEnum.Destroy, new());

        var totalPile = instance.dict[PileEnum.TotalPile];
        var drawPile = instance.dict[PileEnum.Draw];

        foreach (var key in keys)
        {
            totalPile.Add(collection[key]);
            drawPile.Add(collection[key]);
        }
        drawPile.Shuffle();

        return instance;
    }
    public bool Included(PileEnum pile, BattleCard card)
    {
        return dict[pile].Contains(card);
    }
    public CardPile GetPile(PileEnum pile) => dict[pile];
    public void AddTo(PileEnum pile, List<BattleCard> cards)
    {
        var target = dict[pile];
        foreach (var card in cards)
        {
            target.Add(card);
        }
    }
    public void AddTo(PileEnum pile, BattleCard card)
    {
        dict[pile].Add(card);
    }
    /// <summary>塞牌入口：同时进 TotalPile（本场全卡总表）与目标牌堆</summary>
    public void AddCard(BattleCard card, PileEnum pile)
    {
        dict[PileEnum.TotalPile].Add(card);
        dict[pile].Add(card);
    }
    public string CreateCardId()
    {
        var ids = new HashSet<string>();
        foreach (var card in dict[PileEnum.TotalPile].list)
        {
            ids.Add(card.data.id);
        }
        return ids.RandomID();
    }
    /// <summary>
    /// 这个行为会返回list 但不进入手牌区
    /// </summary>
    public List<BattleCard> Draw(int times)
    {
        List<BattleCard> drawList = new();
        var drawPile = dict[PileEnum.Draw];
        var handPile = dict[PileEnum.Hand];

        for (int i = 0; i < times; i++)
        {
            if (handPile.Count() + i == DeckBuildBattleRule.Get().cardLimit) { break; }
            if (drawPile.Count() == 0) { Refill(); }
            if (drawPile.Count() == 0) { break; }

            var drawnCard = drawPile.Index(0);
            drawList.Add(drawnCard);
            drawPile.Remove(drawnCard);
        }
        return drawList;
    }
    public void Refill()
    {
        var drawPile = dict[PileEnum.Draw];
        var discardPile = dict[PileEnum.Discard];

        drawPile.Refill(discardPile);
        drawPile.Shuffle();
        discardPile.Clear();
    }
    public override string ToString()
    {
        var drawPile = dict[PileEnum.Draw];
        return drawPile.ToString();
    }
}
