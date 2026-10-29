using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffCreate 
{
    [SerializeField] private EffectIconEnum effectType;
    public EffectIconEnum EffectType() => effectType;
    public ModifierInfo info;
    public int layer;
    public bool forever;
    public void Apply(IEffectContext context, List<BattleUnit> targetList, out Sequence seq)
    {
        seq = GameAnim.AutoPlay(nameof(BuffCreate));
        foreach (var target in targetList)
        {

        }
    }
}
