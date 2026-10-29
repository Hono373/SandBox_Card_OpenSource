using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BattleIconInfo))]
public class BattleIconInfo : ScriptableObject
{
    public List<Sprite> atkIcons;
    public List<Sprite> shieldIcons;
    public Sprite sleepIcon;
    public Sprite buffIcon;
    public Sprite debuffIcon;
    public Sprite unknowIcon;
    internal static BattleIconInfo Get() => GameAsset.GetSingletonSo<BattleIconInfo>();
    public static Sprite GetIcon(EffectTypeEnum effectTypeEnum)
    {
        switch (effectTypeEnum)
        {
            case EffectTypeEnum.Dmg:
                return BattleIconInfo.Get().atkIcons[0];
            case EffectTypeEnum.Shield:
                return BattleIconInfo.Get().shieldIcons[0];
            case EffectTypeEnum.Sleep:
                return BattleIconInfo.Get().sleepIcon;
            case EffectTypeEnum.Buff:
                return BattleIconInfo.Get().buffIcon;
            case EffectTypeEnum.Debuff:
                return BattleIconInfo.Get().debuffIcon;
            case EffectTypeEnum.Null:
                return BattleIconInfo.Get().unknowIcon;
            default:
                return BattleIconInfo.Get().unknowIcon;
        }
    }
}

