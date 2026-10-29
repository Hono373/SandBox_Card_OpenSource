using DG.Tweening;
public class PressPlayerRoundEnd
{
    public PressPlayerRoundEnd Start(PlayerPhase playerPhase)
    {
        var nextPhase = playerPhase.NextPhase;
        DeckBuildBattle.Active.phase.current = nextPhase;
        GameSerialize.Save();
        nextPhase.Start();
        return this;
    }
}
