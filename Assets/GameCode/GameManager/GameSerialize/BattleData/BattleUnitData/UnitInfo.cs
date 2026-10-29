using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitInfo))]
public class UnitInfo : ScriptableObject, IInfo
{
    [ShowInInspector] public string key => name.TrimPrefix($"{nameof(UnitInfo)}_");

    public string unitName;
    public int maxHP; 
    internal static UnitInfo Get(string key) => GameAsset.GetInstanceSo<UnitInfo>(key);
    public string Key() => key;
    public string Desc() => unitName;
    public BattleUnitGraphicInfo battleUnitObj;
    [SerializeReference] public UnitIntention intention;
    public List<Skill> battleStartSkills = new();

    public void OnValidate()
    {
       
    }
    [Button("验证数据")]
    public void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(unitName))
            Debug.LogError($"单位 {name} 名称为空");
        if (maxHP <= 0)
            Debug.LogError($"单位 {name} maxHP 小于等于 0");
        if (battleUnitObj == null)
            Debug.LogError($"单位 {name} battleUnitObj 为空");
        if (intention == null)
            Debug.LogError($"单位 {name} intention 为空");
        else
            intention.ValidateData();
        foreach (var skill in battleStartSkills)
        {
            if (skill == null)
                Debug.LogError($"单位 {name} battleStartSkills 存在 null skill");
            else
                skill.ValidateData();
        }
        Debug.Log($"单位 {name} 数据验证完成");
    }
}
