using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleUnitModifierBarUI : MonoBehaviour
{
    BattleUnitUI obj;
    readonly List<ModifierObj> modifierObjs = new();

    public void Init(BattleUnitUI unitObj)
    {
        this.obj = unitObj;
    }

    public ModifierObj InstantiateObj(Modifier modifier)
    {
        var obj = GameAsset.Instantiate<ModifierObj>(transform);
        obj.Init(modifier);
        modifierObjs.Add(obj);
        return obj;
    }
    public void AddOrRefresh(Modifier modifier)
    {
        for (int i = 0; i < modifierObjs.Count; i++)
        {
            if (modifierObjs[i].Id != modifier.data.id) continue;
            if (modifierObjs[i].Refresh()) return;
            // 图标在 Refresh 里因 layer==0 自销毁了，从列表摘掉
            modifierObjs.RemoveAt(i);
            return;
        }
        InstantiateObj(modifier);
    }
    /// <summary>
    /// 移除一个 buff 图标。2026-10-22 新增 —— 原来只有 AddOrRefresh，没有移除入口，
    /// 导致「临时 buff 到期」（活动肌肉的力量下降、双发、狂怒）数据删了但图标一直挂在界面上。
    /// </summary>
    public void RemoveModifier(Modifier modifier)
    {
        for (int i = modifierObjs.Count - 1; i >= 0; i--)
        {
            if (modifierObjs[i].Id != modifier.data.id) continue;
            Destroy(modifierObjs[i].gameObject);
            modifierObjs.RemoveAt(i);
            return;
        }
    }
    internal void AddModifier(Modifier modifier, out Sequence addModifier)
    {
        addModifier = GameAnim.AutoPlay(nameof(AddModifier));
        addModifier.PrependCallback(() => InstantiateObj(modifier));
    }
    /// <summary>全量重建 buff 栏（2026-10-22 补：原来直接遍历 InstantiateObj，没清旧图标，会越刷越多）</summary>
    internal void Refresh()
    {
        foreach (var modifierObj in modifierObjs)
        {
            Destroy(modifierObj.gameObject);
        }
        modifierObjs.Clear();

        foreach (var modifier in obj.manager.data.modifiers)
        {
            if (modifier.data.layer == 0) continue;
            InstantiateObj(modifier);
        }
    }
}
