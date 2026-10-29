using DG.Tweening;
using Newtonsoft.Json;

public class BattlePhaseEnd : BattlePhase
{
    public override BattlePhase NextPhase => null;
    public override BattlePhase Start()
    {
        Init();
        return this;
    }
}
