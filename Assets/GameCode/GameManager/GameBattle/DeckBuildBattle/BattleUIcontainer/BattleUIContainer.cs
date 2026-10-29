using System;
using DG.Tweening;
using UnityEngine;

public class BattleUIContainer : MonoBehaviour
{
    DeckBuildBattleUI top;
    UIGroup uiGroup;
    [HideInInspector] public BattleRoundEndBtn battleRoundEndBtn;
    [HideInInspector] public BattleDrawPileCheckBtn drawPilecheckBtn;
    [HideInInspector] public BattleDiscardPileCheckBtn discardPileCheckBtn;
    [HideInInspector] public CostContainer costContainer;
    internal void Init(DeckBuildBattle ddb)
    {
        UIGroup.Create(this, out uiGroup);
        this.CheckInChild(out battleRoundEndBtn).Init(ddb);
        this.CheckInChild(out drawPilecheckBtn).Init(ddb);
        this.CheckInChild(out discardPileCheckBtn).Init(ddb);
        this.CheckInChild(out costContainer).Init(ddb);
    }
    internal void SetRoundEndBtnActive(bool v)
    {
        battleRoundEndBtn.SetActive(v);
    }
    internal void SetRoundEndBtnInteractable(bool v)
    {
        battleRoundEndBtn.SetInteractable(v);
    }
    internal Sequence RefreshCostUI(int currentCost, int maxCost)
    {
        return costContainer.RefreshUI(currentCost, maxCost);
    }
    /// <summary>
    /// 2026-10-23 牌堆计数同步：抽牌堆 / 弃牌堆按钮上的数字都从 deck 重读。
    /// 之前只有抽牌堆在个别 flow 里单独刷，弃牌堆从来没有更新过。
    /// </summary>
    internal void RefreshPileCount()
    {
        drawPilecheckBtn.Refresh();
        discardPileCheckBtn.Refresh();
    }
}
