using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 实现拖拽与指针释放两种模式，和一种属性获取器
/// </summary>
[Serializable]
public abstract class BattleCardInteractive
{
    public void OnPointerEnter(BattleCardUI obj, PointerEventData eventData)
    {
        if (DeckBuildBattle.GetUI().handContainer.CardDrag) return;
        if (obj.stateEnum != CardStateEnum.Idle) return;
        BattleCardFocusProcess(obj);
    }
    public void BattleCardFocusProcess(BattleCardUI obj)
    {
        obj.stateEnum = CardStateEnum.Focus;
        obj.transform.SetAsLastSibling();
        obj.animOverride.Override(CardFocusAnim(obj), needComplete: true);
        CardPushAnim(obj);
    }
    Sequence CardFocusAnim(BattleCardUI obj)
    {
        var config = BattleCardObjAnim.Get();
        var y = -DeckBuildBattle.GetUI().handContainer.RectT().rect.height / 2 + config.cardHeight * config.focus.scale / 2;
        var pos = new Vector2(obj.manager.argData.pos.x, y);

        return obj.Motion(pos, Quaternion.identity, config.focus.scale * Vector3.one, config.focus.moveSpd, nameof(CardFocusAnim));
    }
    Sequence CardPushAnim(BattleCardUI obj)
    {
        var config = BattleCardUI.Anim;
        var handContainer = DeckBuildBattle.GetUI().handContainer;
        var handList = DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list;

        var selfIndex = handList.IndexOf(obj.manager);
        var seq = GameAnim.AutoPlay(nameof(CardPushAnim));
        foreach (var otherCard in handList)
        {
            if (otherCard == obj.manager) continue;
            var otherUI = handContainer.GetCardUI(otherCard);

            var indexOffset = handList.IndexOf(otherCard) - selfIndex;
            var amount = config.push.pushOffset * Mathf.Pow(0.25f, Mathf.Abs(indexOffset) - 1);
            var pos = otherCard.argData.pos + new Vector2(amount * Mathf.Sign(indexOffset), 0);

            otherUI.animOverride.Override(otherUI.Motion(pos, otherCard.argData.rotate, Vector3.one, config.push.pushSpd, nameof(CardPushAnim)), needComplete: false);
        }
        return seq;
    }
    public bool OnPointerDownCheck(BattleCardUI obj, PointerEventData eventData)
    {
        if (DeckBuildBattle.Active.phase.current is not PlayerPhase) return false;
        if (eventData.button != PointerEventData.InputButton.Left) return false;
        if (DeckBuildBattle.GetUI().handContainer.CardDrag) return false;

        if (obj.stateEnum == CardStateEnum.Idle)
        {
            BattleCardFocusProcess(obj);
        }

        if (obj.manager.modified.useBlock)
        {
            DeckBuildBattle.GetUI().messageContainer.Message(obj.manager.modified.useBlockDesc);
            return false;
        }

        if (obj.manager.modified.costResult > DeckBuildBattle.Active.data.energy)
        {
            DeckBuildBattle.GetUI().messageContainer.Message("费用不足");
            return false;
        }

        DeckBuildBattle.GetUI().handContainer.CardDrag = true;
        obj.stateEnum = CardStateEnum.Drag;
        DeckBuildBattle.GetUI().uiContainer.SetRoundEndBtnInteractable(false);
        return true;
    }
    public abstract void OnPointerDown(BattleCardUI obj, PointerEventData eventData);
    public bool DragCheck(BattleCardUI obj, PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return false;
        if (obj.stateEnum != CardStateEnum.Drag) return false;
        return true;
    }
    public abstract void Drag(BattleCardUI obj, PointerEventData eventData);
    public bool OnPointerUpCheck(BattleCardUI obj, PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return false;
        if (obj.stateEnum != CardStateEnum.Drag) return false;
        if (DeckBuildBattle.Active.phase.current is not PlayerPhase) return false;
        return true;
    }
    public abstract void OnPointerUp(BattleCardUI obj, PointerEventData eventData);
    public void OnPointerExit(BattleCardUI obj, PointerEventData eventData)
    {
        if (DeckBuildBattle.GetUI().handContainer.CardDrag) return;
        if (obj.stateEnum == CardStateEnum.Destroy) return;
        SetIdle(obj, true);
        DeckBuildBattle.GetUI().handContainer.Arrange();

    }
    public void SetIdle(BattleCardUI ui, bool forceChangeStats, bool immediately = false)
    {
        if (!forceChangeStats)
        {
            if (ui.stateEnum != CardStateEnum.Idle)
                return;
        }

        ui.SetOriginalSiblingIndex();
        ui.stateEnum = CardStateEnum.Idle;
        if (immediately)
        {
            ui.RectTransform().anchoredPosition = ui.manager.argData.pos;
            ui.RectTransform().rotation = ui.manager.argData.rotate;
            ui.RectTransform().localScale = ui.manager.argData.scale;
        }
        else
        {
            ui.animOverride.Override(ui.IdleAnim(), false);
        }
    }

    public abstract BattleCardInteractive Create(BattleCard card);
    internal virtual void ValidateData() { }

    public void OnBeginDrag(BattleCardUI obj, PointerEventData eventData)
    {
    }

    public void OnEndDrag(BattleCardUI obj, PointerEventData eventData)
    {
    }

    public abstract BattleCardInteractive Create();
}
