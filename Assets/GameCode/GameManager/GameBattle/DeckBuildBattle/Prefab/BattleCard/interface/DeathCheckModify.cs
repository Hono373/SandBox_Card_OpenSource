using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class DeathCheckModify : GameModify
{
    public bool skipDeathCheck;
    public BattleUnit unit;
    public BattleEndCheckFlow check;
    public DeathCheckModify Start(BattleEndCheckFlow check, BattleUnit unit, bool needSettle)
    {
        this.check = check;
        this.unit = unit;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        if (unit.current.isDead) return;
        if (skipDeathCheck) return;
        if (unit.data.hp <= 0 || unit.current.nearDeath)
        {
            unit.current.isDead = true;
            check.deathUnits.Add(unit);
        }
    }
}