using UnityEngine;

[CreateAssetMenu(fileName = nameof(BattleUnitObjInfo))]
public class BattleUnitObjInfo : ScriptableObject
{
    public Sprite planNone;
    public static BattleUnitObjInfo Get()
    {
        return GameAsset.GetSingletonSo<BattleUnitObjInfo>();
    }
}