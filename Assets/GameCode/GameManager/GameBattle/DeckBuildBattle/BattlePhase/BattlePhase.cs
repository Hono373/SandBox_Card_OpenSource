using System;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
[Serializable]
public abstract class BattlePhase : RootBattleFlow
{
  public abstract BattlePhase NextPhase { get; }
  public abstract BattlePhase Start();
}
