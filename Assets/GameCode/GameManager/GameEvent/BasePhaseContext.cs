using System;
public class BasePhaseContext
{
    public Func<BattlePhase> DefaultNextPhase { protected set; get; }
    public SortedWeight<BattlePhase> nextPhase = new();
    public BattlePhase NextPhase() => nextPhase.MaxOrDefault(DefaultNextPhase);
}
   