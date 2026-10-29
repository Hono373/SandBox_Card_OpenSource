using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ModifierInfo))]
public class ModifierInfo : ScriptableObject
{
    [ShowInInspector] public string key => name.TrimPrefix($"{nameof(ModifierInfo)}_");
    /// <summary>图标按资源命名契约 Modifier_{key} 加载（与卡牌 Card_{key} 一致）</summary>
    public Sprite Img
    {
        get
        {
            // 2026-10-21 必须用 Unity 重载的 == 判空：??= 在 IL 层只看真实引用，
            // 上一轮 Play 卸载后残留的"已销毁 Sprite"会被判为非空，导致永远返回假 null。
            if (cachedImg == null)
                cachedImg = GameAsset.GetSprite($"{nameof(Modifier)}_{key}");
            GameDebug.Log($"ModifierInfo.Img: key={key} → {(cachedImg == null ? "null" : cachedImg.name)}");
            return cachedImg;
        }
    }
    Sprite cachedImg;
    public string modifierName;
    /// <summary>同 key 只保留一个实例，再次施加改为叠加层数</summary>
    public bool only;
    /// <summary>负面效果，会被人工制品等拦截</summary>
    public bool debuff;
    /// <summary>不在单位 buff 栏显示（内部被动用）</summary>
    public bool hidden;
    [SerializeReference] public List<ModifierEffect> modifierEffects = new();
    public static ModifierInfo Get(string key) => GameAsset.GetInstanceSo<ModifierInfo>(key);
    /// <summary>
    /// 拼出 buff 的完整描述。各 ModifierEffect 的 desc 里用 {0} 表示层数（如力量："造成的伤害增加{0}"），
    /// 2026-10-22 在此统一格式化 —— 之前直接返回拼接结果，UI 上会看到字面量 "{0}"。
    /// </summary>
    public string GetDescription(Modifier buff)
    {
        var description = string.Empty;
        foreach (var info in modifierEffects)
        {
            description += info.GetDescription(buff);
        }
        if (description.Length == 0) return description;
        return string.Format(description, buff.data.layer);
    }
    public void IsActive(Modifier modifier)
    {
        foreach (var info in modifierEffects)
        {
            info.IsActive(modifier);
        }
    }
    public void Settle(Modifier modifier)
    {

    }
    public void OnValidate()
    {
    }
    [Button("验证数据")]
    public void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(modifierName))
            Debug.LogError($"修饰 {name} 名称为空");
        if (modifierEffects == null || modifierEffects.Count == 0)
            Debug.LogError($"修饰 {name} modifierEffects 为空");
        else
            foreach (var effect in modifierEffects)
            {
                if (effect == null || effect.GetType().IsAbstract)
                    Debug.LogError($"修饰 {name} 存在无效 modifierEffect（null 或抽象类）");
                else
                    effect.ValidateData();
            }
        Debug.Log($"修饰 {name} 数据验证完成");
    }
}