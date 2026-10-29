using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HandCardCombineFlow : BattleFlow
{
    public List<BattleCard> combines = new();
    public List<MotionData> combineData = new();
    public HandDrawAnim config;
    public List<RefreshDrawPilecheckUIFlow> refreshDrawPilecheckUIFlows = new();
    public HandCardCombineFlow Start(ICollection<BattleCard> cards, List<MotionData> combineData)
    {
        combines.AddRange(cards);
        this.combineData = combineData;
        config = BattleCardUI.Anim.handDraw;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        for (var i = 0; i < combines.Count; i++)
        {
            var card = combines[i];
            var ui = DeckBuildBattle.GetUI().handContainer.InstantiateObj(card);
            ui.Init(card);
            ui.stateEnum = CardStateEnum.Display;
            ui.RectTransform().anchoredPosition = config.startPos;
            ui.CanvasGroup().Ray(false);
            ui.CanvasGroup().Alpha(true);

            var createToDisplay = new BattleCardUIMotion().Start(ui, config.data, config.createToDisplaySpd);
            var displayToHand = new BattleCardUIMotion().Start(ui, combineData[i], config.displayToHandSpd);

            var refreshDrawPilecheckUIFlow = new RefreshDrawPilecheckUIFlow().Start(combines.Count, i + 1, DeckBuildBattle.Active.deck.GetPile(PileEnum.Draw).list.Count);
            refreshDrawPilecheckUIFlows.Add(refreshDrawPilecheckUIFlow);
            var oneCombine = GameAnim.Flow(nameof(HandDrawAnim));
            oneCombine.Append(createToDisplay.flow);
            oneCombine.AppendInterval(config.displayTime);
            oneCombine.Append(displayToHand.flow);

            oneCombine.onComplete += () =>
            {
                ui.stateEnum = CardStateEnum.Idle;
                ui.SetOriginalSiblingIndex();
                ui.CanvasGroup().Ray(true);
                ui.animOverride.Start();
            };

            flow.Insert((config.createToDisplaySpd + config.displayTime) * i, refreshDrawPilecheckUIFlow.flow);
            flow.Insert((config.createToDisplaySpd + config.displayTime) * i, oneCombine);
        }
    }
}