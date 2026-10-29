using System;
using System.Collections.Generic;

public class ModifierInRoundSnapshot
{
    Type currentRound;
    HashSet<BattleUnit> effectOn;
    ModifierInRoundSnapshot() { }
    public ModifierInRoundSnapshot(List<BattleUnit> effectOn) { this.effectOn = new HashSet<BattleUnit>(effectOn); }
}
