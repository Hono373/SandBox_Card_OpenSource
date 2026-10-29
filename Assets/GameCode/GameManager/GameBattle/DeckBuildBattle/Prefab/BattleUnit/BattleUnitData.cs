using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleUnitData
{
    public string id;
    public string key;
    public int maxHP;
    public int hp;
    public int shield;
    [SerializeReference] public List<Modifier> modifiers = new();


    BattleUnitData() { }
    public static BattleUnitData Create(Unit unit, CampEnum team)
    {
        var instance = new BattleUnitData();
        instance.id = unit.id;
        instance.key = unit.key;
        instance.maxHP = unit.data.maxHP;
        instance.hp = unit.data.hp;
        return instance;
    }
}
