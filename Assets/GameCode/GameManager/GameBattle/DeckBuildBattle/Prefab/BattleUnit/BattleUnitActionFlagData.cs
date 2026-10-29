using System;
using UnityEngine;

[Serializable]
public class BattleUnitActionFlagData
{
    public Sprite sprite;
    public int value;
    public bool active;
    public BattleUnitActionFlagData() { }
    public BattleUnitActionFlagData(Sprite sprite, int value, bool active) { this.sprite = sprite; this.value = value; this.active = active; }
}

