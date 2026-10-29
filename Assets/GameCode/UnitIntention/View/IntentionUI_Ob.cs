using UnityEngine;

public abstract class IntentionUI_Ob
{
    public abstract void Refresh(IntentionUIData_Ob data);

    public Sprite GetSprite(IntentionUIData_Ob data)
    {
        switch (data.type)
        {
            case EffectIconEnum.Atk:
                if (!data.value.HasValue)
                {
                    GameDebug.Log("BattleEffectType.atk value is null");
                    return null;
                }
                if (BattleIconInfo.Get().atkIcons.Count <= (data.value.Value / 10).GreaterThanOrEqual())
                {
                    return BattleIconInfo.Get().atkIcons[^1];
                }
                return BattleIconInfo.Get().atkIcons[data.value.Value / 10];
            default:
                return GameAsset.GetSprite("Unknown");
        }
    }
}
