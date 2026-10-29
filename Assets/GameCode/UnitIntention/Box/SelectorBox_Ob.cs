using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 具体实现由胶水层决定
/// </summary>
[Serializable]
public class SelectorBox_Ob
{
    [InfoBox("通过设置条件，删除不符合条件的单位")]
    [SerializeReference] public List<IUnitSelector> selector = new();
    public void Trim(List<string> targetList)
    {
        foreach (var item in selector)
        {
            item.Trim(targetList);
        }
    }
}
