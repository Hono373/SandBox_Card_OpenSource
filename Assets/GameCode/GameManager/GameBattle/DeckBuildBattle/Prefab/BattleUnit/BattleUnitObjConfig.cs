using UnityEngine;

[CreateAssetMenu(fileName = nameof(BattleUnitObjConfig))]
public class BattleUnitObjConfig : ScriptableObject
{
    public static BattleUnitObjConfig Get()
    {
        return GameAsset.GetSingletonSo<BattleUnitObjConfig>();
    }
    public GameObject battleUnitPrefab;
}