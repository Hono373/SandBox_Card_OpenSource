using System;
using System.Collections.Generic;

[Serializable]
public class UnitData
{
    public int maxHP;
    public int hp;
    public Dictionary<string, Modifier> modifierDict = new();
    public UnitData() { }
    public UnitData(UnitInfo info)
    {
        hp = maxHP = info.maxHP;
    }
}
