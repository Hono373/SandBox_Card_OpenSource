using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class WinnerCheckPhase : BattleFlow
{
    public BattleEndCheckFlow check;
    public bool isMateDefeat = true;
    public bool isEnemyDefeat = true;
    public bool battleIsEnd = false;
    public WinnerCheckPhase Start(BattleEndCheckFlow check)
    {
        this.check = check;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        foreach (var unit in DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Mate))
        {
            if (!unit.current.isDead)
            {
                isMateDefeat = false;
                break;
            }
        }
        foreach (var unit in DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Enemy))
        {
            if (!unit.current.isDead)
            {
                isEnemyDefeat = false;
                break;
            }
        }

        if (isMateDefeat || isEnemyDefeat)
        {
            DeckBuildBattle.Active.isGameOver = true;
            DeckBuildBattle.Active.phase.current = new BattlePhaseEnd().Start();
            DeckBuildBattle.GetUI().uiContainer.SetRoundEndBtnActive(false);

            if (isMateDefeat)
            {
                TweenCallback action = () => DeckBuildBattle.GetUI().settleContainer.Message(BattleResultEnum.PlayerLose);
                flow.onComplete += action;
            }
            if (isEnemyDefeat)
            {
                TweenCallback action = () => DeckBuildBattle.GetUI().settleContainer.Message(BattleResultEnum.PlayerWin);
                flow.onComplete += action;
            }
        }
    }
}