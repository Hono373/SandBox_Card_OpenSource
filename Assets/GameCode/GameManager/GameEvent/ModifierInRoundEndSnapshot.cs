using System.Collections.Generic;

public class ModifierInRoundEndSnapshot
{
    HashSet<BattleUnit> effectOn;
    ModifierInRoundEndSnapshot() { }
    public ModifierInRoundEndSnapshot(List<BattleUnit> effectOn) { this.effectOn = new HashSet<BattleUnit>(effectOn); }
}
