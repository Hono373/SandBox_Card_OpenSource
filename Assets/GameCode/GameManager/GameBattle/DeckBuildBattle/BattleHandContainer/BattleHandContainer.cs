using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class BattleHandContainer : MonoBehaviour
{
    private RectTransform rectT;
    public bool CardDrag;
    readonly Dictionary<string, BattleCardUI> cardObjDict = new();

    public RectTransform RectT() => rectT ??= this.GetComponentSafety<RectTransform>(out var rt);
    internal void Init(DeckBuildBattle battle)
    {
        LoadData(battle);
    }
    void LoadData(DeckBuildBattle ddb)
    {
        foreach (var card in DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list)
        {
            var obj = InstantiateObj(card);
            obj.CanvasGroup().SetActive(true);
            obj.manager.modified.interactiveMode.SetIdle(obj, true, true);
        }
    }
    public BattleCardUI InstantiateObj(BattleCard card)
    {
        if (cardObjDict.TryGetValue(card.data.id, out var exist))
        {
            GameDebug.Log("BattleCardObj 已存在，错误调用！");
            return exist;
        }
        var obj = GameAsset.Instantiate<BattleCardUI>(transform);
        obj.Init(card);
        cardObjDict[card.data.id] = obj;
        return obj;
    }
    public BattleCardUI GetCardUI(BattleCard card)
    {
        if (!cardObjDict.TryGetValue(card.data.id, out var obj))
        {
            GameDebug.Log("BattleCardObj 不存在，错误调用！");
        }
        return obj;
    }
    public void RecycleObj(BattleCard card)
    {
        cardObjDict.Remove(card.data.id);
    }
    public void Arrange()
    {
        var handList = DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list;
        foreach (var card in handList)
        {
            card.modified.interactiveMode.SetIdle(GetCardUI(card), false);
        }
    }
    public void CalculateArg()
    {
        var handList = DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list;
        var arrangeList = BattleCardUI.Anim.handArrangeCalculation.Result(handList.Count);
        for (var i = 0; i < handList.Count; i++)
        {
            handList[i].argData = arrangeList[i];
        }
    }
}
