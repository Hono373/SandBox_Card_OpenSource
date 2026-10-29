using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
public class BattleUnitCurrent : GameContext
{
    public BattleUnit target;
    public BattleUnit owner;
    public bool nearDeath;
    public bool isDead;

    internal static BattleUnitCurrent Create(BattleUnit unit)
    {
        var instance = new BattleUnitCurrent();
        instance.owner = unit;

        return instance;
    }
    public void Modify()
    {

    }
}