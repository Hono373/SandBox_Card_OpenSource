using System;
using System.Collections.Generic;
using DG.Tweening;

public static class GameModifyCenter
{
    public static Dictionary<string, Modifier> modifiers = new();
    public static void AddTo(Modifier modifier, List<Modifier> modifiers)
    {
        modifiers.Add(modifier);
        GameModifyCenter.modifiers[modifier.data.id] = modifier;
    }
    public static string CreateID() => modifiers.Keys.RandomID();
    public static void Register(Modifier modifier)
    {
        modifiers[modifier.data.id] = modifier;
    }

    /// <summary>从全局管理器注销 modifier 实例</summary>
    public static void RemoveRegister(Modifier modifier)
    {
        modifiers.Remove(modifier.data.id);
    }
    public static void Modify(GameModify modifyEvent, bool needSettle)
    {
        // 快照遍历：Settle 已改为延迟到消费后（GameModify.Settle），但 Modify 内部仍可能间接触发
        // 增删 modifier（嵌套 flow / UI 刷新），直接遍历 modifiers.Values 会抛 Collection was modified
        var snapshot = new List<Modifier>(modifiers.Values);
        foreach (var modifier in snapshot)
        {
            if (!modifiers.ContainsKey(modifier.data.id)) continue; // 兜底：本轮内已被移除则不再参与结算
            foreach (var effect in modifier.modifierEffects)
            {
                effect.Modify(modifier, modifyEvent, needSettle);
            }
        }
    }

}