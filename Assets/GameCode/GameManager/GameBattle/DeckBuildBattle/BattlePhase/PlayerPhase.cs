using System;
using DG.Tweening;

[Serializable]
public class PlayerPhase : BattlePhase
{
    public override BattlePhase NextPhase => new PlayerPhaseEnd();
    public override BattlePhase Start()
    {
        DeckBuildBattle.Active.phase.current = this;
        GameSerialize.Save();
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var topObj = DeckBuildBattle.GetUI();
        flow.onComplete += () => topObj.messageContainer.Message("Player's Turn");
        flow.onComplete += () => topObj.uiContainer.SetRoundEndBtnActive(true);
    }
}
