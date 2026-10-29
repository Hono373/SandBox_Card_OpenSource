using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntentionModuleUIListener_Ob
{
    [JsonProperty] public HashSet<IIntentionModuleUI_Ob> uiSet = new();
    public void Register(IIntentionModuleUI_Ob listener)
    {
        if (!uiSet.Add(listener))
            GameDebug.Log($"{listener.GetType().Name} already exists");
    }
    public void RemoveRegister(IIntentionModuleUI_Ob listener)
    {
        if (!uiSet.Remove(listener))
            GameDebug.Log($"{listener.GetType().Name} does not exist");
    }
    public void OnValueChange(List<IntentionUIData_Ob> datas, Sequence sequence)
    {
        foreach (var listener in uiSet)
        {
            listener.OnValueChange(datas, sequence);
        }
    }
}
