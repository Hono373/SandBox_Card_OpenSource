using DG.Tweening;
using DG.Tweening.Core;
using System.Collections.Generic;
using UnityEngine;

public class BattlePopValueContainer : MonoBehaviour
{
    [SerializeReference] float duration = 0.35f;
    [SerializeField] Queue<PopValueObj> pool = new();
    public void Init(DeckBuildBattle ddb)
    {
        pool.Enqueue(PopValueObj.Create(transform));
        pool.Enqueue(PopValueObj.Create(transform));
        pool.Enqueue(PopValueObj.Create(transform));
        pool.Enqueue(PopValueObj.Create(transform));
        pool.Enqueue(PopValueObj.Create(transform));
    }
    public void PopValue(BattleUnitUI target, int value, PopValueObj.ValueColor color, out Sequence popValue)
    {
        popValue = GameAnim.AutoPlay(nameof(PopValue));

        PopValueObj item;

        if (pool.Count != 0)
        {
            item = pool.Dequeue();
        }
        else
        {
            item = PopValueObj.Create(transform);
        }

        item.ShowPop(target, value, color, duration, out var showPop);
        popValue.Join(showPop);
    }
    /// <summary>通用数值弹字：按结果里的 damage/heal/shieldLost 分别弹红/绿/蓝字（为 0 的字段不弹）。</summary>
    public void PopValue(BattleUnitUI target, SkillEffectResult result, out Sequence popValue)
    {
        popValue = GameAnim.AutoPlay(nameof(PopValue));

        if (result.damage > 0)
        {
            PopValue(target, result.damage, PopValueObj.ValueColor.Red, out var damagePop);
            popValue.Join(damagePop);
        }
        if (result.heal > 0)
        {
            PopValue(target, result.heal, PopValueObj.ValueColor.Green, out var healPop);
            popValue.Join(healPop);
        }
        if (result.shieldLost > 0)
        {
            PopValue(target, result.shieldLost, PopValueObj.ValueColor.Blue, out var shieldPop);
            popValue.Join(shieldPop);
        }
    }
    internal void Recycle(PopValueObj popValue)
    {
        popValue.uiGroup.Alpha(false);
        pool.Enqueue(popValue);
    }
}

