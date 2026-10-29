using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class BattleCardInteractive_Choice : BattleCardInteractive
{
    public override void OnPointerDown(BattleCardUI obj, PointerEventData eventData)
    {
        if (!OnPointerDownCheck(obj, eventData)) return;

        DeckBuildBattle.GetUI().arrowContainer.PrepareForDrag(obj.RectTransform().position);
        var choiceFlow = new CardChoiceFlow().Start(obj);
        obj.animOverride.Override(choiceFlow.flow, true);
    }
    public static GameObject previousChoice;
    public override void Drag(BattleCardUI obj, PointerEventData eventData)
    {
        if (!DragCheck(obj, eventData)) return;
        var dbb = DeckBuildBattle.GetUI();
        var card = obj.manager;

        //指针跟踪
        dbb.arrowContainer.arrowLine.Drag(obj.RectTransform().anchoredPosition, obj.GetMousePos());

        //获取指针选中物体
        var inChoice = eventData.pointerCurrentRaycast.gameObject;
        //如果和上次指向物体一样就不再次计算
        if (inChoice == previousChoice) return;
        //更新缓存，避免下一帧重复计算
        previousChoice = inChoice;

        //试图从指针物体获取mask后获取BattleUnit
        bool getUnit = GetChoiceTarget(inChoice, out BattleUnit target);
        //进行获取到的角色的可指向性判定
        if (getUnit && card.modified.rangeRule.IsTrueChoice(card.modified.owner, target))
        {
            //指针外观设置为指向了单位
            dbb.arrowContainer.arrowLine.State = ArrowStateEnum.Targeted;
            new BattleCardPremodify_OnChoice().Start(card.modified, target);
        }
        else
        {
            //将卡牌描述设置为非指向时仅计算自身的状态
            new RefreshBattleCardGroupFlow().Start(needSettle: false).flow.Play();
            //设置为无指向外观
            dbb.arrowContainer.arrowLine.State = ArrowStateEnum.NoTarget;
            obj.cardDesc.text = card.modified.Desc();
        }
    }
    bool GetChoiceTarget(GameObject inChoice, out BattleUnit target)
    {
        target = null;
        if (inChoice == null) return false;
        if (!inChoice.CompareTag("UnitMask")) return false;
        target = inChoice.GetComponent<BattleUnitSelectedUI>().GetUnit();
        return true;
    }
    public override void OnPointerUp(BattleCardUI ui, PointerEventData eventData)
    {
        if (OnPointerUpCheck(ui, eventData))
        {
            var card = ui.manager;
            //如果获取的目标单位是一个可指向的有效单位
            if (GetChoiceTarget(eventData.pointerCurrentRaycast.gameObject, out BattleUnit choice) && card.modified.rangeRule.IsTrueChoice(card.modified.owner, choice))
            {
                var battleCardUsePhase = new BattleCardUsePhaseFlow().Start(card, choice);
            }
            else
            {
                DeckBuildBattle.GetUI().messageContainer.Message("无效的目标");
                DeckBuildBattle.GetUI().arrowContainer.arrowLine.Alpha(false);
            }
        }
        DeckBuildBattle.GetUI().handContainer.CardDrag = false;
        DeckBuildBattle.GetUI().uiContainer.SetRoundEndBtnInteractable(true);
        if (ui.stateEnum == CardStateEnum.Destroy) return;
        SetIdle(ui, true);
        ui.Useable();
    }
    public override BattleCardInteractive Create(BattleCard card) => new BattleCardInteractive_Choice();
    public override BattleCardInteractive Create() => new BattleCardInteractive_Choice();
}


