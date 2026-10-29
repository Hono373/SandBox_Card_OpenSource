using System;
using DG.Tweening;
using Newtonsoft.Json;

[Serializable]
public abstract class BattleFlow
{
    [JsonIgnore] public Sequence flow;
    protected virtual void Init()
    {
        flow = GameAnim.Flow(GetType().Name);
        if (DeckBuildBattle.Active.isGameOver) return;
        FlowBuild();
    }
    protected virtual void FlowBuild() { }
}



