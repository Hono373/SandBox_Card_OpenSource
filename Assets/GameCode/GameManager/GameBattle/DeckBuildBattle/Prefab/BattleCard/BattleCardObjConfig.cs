using UnityEngine;

[CreateAssetMenu(fileName = nameof(BattleCardObjConfig))]
public class BattleCardObjConfig : ScriptableObject
{
    public static BattleCardObjConfig Get()
    {
        return GameAsset.GetSingletonSo<BattleCardObjConfig>();
    }
}