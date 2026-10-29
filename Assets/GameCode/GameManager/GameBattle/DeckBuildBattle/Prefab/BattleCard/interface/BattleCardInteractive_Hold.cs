
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class BattleCardInteractive_Hold : BattleCardInteractive
{
    public BattleCardInteractive_Hold() { }
    public override void OnPointerDown(BattleCardUI obj, PointerEventData eventData)
    {
        if (!OnPointerDownCheck(obj, eventData)) return;
    }
    public override void Drag(BattleCardUI obj, PointerEventData eventData)
    {
        if (!DragCheck(obj, eventData)) return;
        var card = obj.manager;
        obj.RectTransform().anchoredPosition = obj.GetMousePos();

        if (obj.RectTransform().anchoredPosition.y - card.argData.pos.y > BattleCardObjAnim.Get().hold.rangeToUse)
        {
            obj.InUseRange();
        }
        else
        {
            obj.Useable();
        }
    }
    public override void OnPointerUp(BattleCardUI obj, PointerEventData eventData)
    {
        if (OnPointerUpCheck(obj, eventData))
        {
            var card = obj.manager;
            //卡牌的位置超过阈值后才能打出
            var avaliableToUse = obj.RectTransform().anchoredPosition.y - card.argData.pos.y > BattleCardObjAnim.Get().hold.rangeToUse;
            if (avaliableToUse)
            {
                var battleCardUsePhase = new BattleCardUsePhaseFlow().Start(card, null);
            }
            else
            {

            }
        }
        DeckBuildBattle.GetUI().handContainer.CardDrag = false;
        DeckBuildBattle.GetUI().uiContainer.SetRoundEndBtnInteractable(true);
        if (obj.stateEnum == CardStateEnum.Destroy) return;
        SetIdle(obj, true);
        obj.Useable();
    }
    public override BattleCardInteractive Create(BattleCard card) => new BattleCardInteractive_Hold();
    public override BattleCardInteractive Create() => new BattleCardInteractive_Hold();

}
