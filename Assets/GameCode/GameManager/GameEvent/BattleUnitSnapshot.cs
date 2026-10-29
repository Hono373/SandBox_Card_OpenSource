using System.Collections.Generic;

public class BattleUnitSnapshot
{
    public Unit unit;
    public int maxHPFix;
    public int HPFix;
    public List<Modifier> addList = new();
    BattleUnitSnapshot() { }
    public BattleUnitSnapshot(Unit unit) { this.unit = unit; }
}
