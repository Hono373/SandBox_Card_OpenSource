using DG.Tweening;
using System;
using UnityEngine;
using System.Collections.Generic;
public class BattleUnitModifierModuleUI : MonoBehaviour
{
    [SerializeField] Dictionary<string, ModifierObj> modifiers = new();
    Queue<ModifierObj> pool = new();
    public void Deserialize(List< Modifier> source)
    {
        foreach (var modifier in source)
        {
            InstantiateObj(modifier);
        }
    }
    public void Init(BattleUnitUI unitObj)
    {
        Deserialize(unitObj.manager.data.modifiers);
    }
    public void InstantiateObj(Modifier modifier)
    {
        var obj = pool.Count > 0 ? pool.Dequeue() : GameAsset.Instantiate<ModifierObj>(nameof(ModifierObj), transform);
        modifiers[modifier.data.id] = obj;
        obj.Init(modifier);
    }
    public void OnAdd(Modifier modifier, Sequence seq)
    {
        if (modifiers.TryGetValue(modifier.data.id, out var obj))
        {
            obj.OnAdd(seq);
        }
    }
    public void OnRemove(Modifier modifier, Sequence seq)
    {
        if (modifiers.Remove(modifier.data.id, out var obj))
        {
            obj.OnRemove(seq);
        }
        seq.OnComplete(() => { obj.Recovery(); pool.Enqueue(obj); });
    }
    public void OnValueChange(Modifier modifier, Sequence seq)
    {
        if (modifiers.TryGetValue(modifier.data.id, out var obj))
        {
            obj.OnValueChange(seq);
        }
    }
}
