using UnityEngine;
[CreateAssetMenu(fileName = nameof(BattleUnitObjAnime))]
public class BattleUnitObjAnime : ScriptableObject
{
    public static BattleUnitObjAnime Get()
    {
        return GameAsset.GetSingletonSo<BattleUnitObjAnime>();
    }
    public GameObject battleUnitPrefab;
}
