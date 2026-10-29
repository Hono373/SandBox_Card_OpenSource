using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = nameof(DeckBuildBattleInfo))]
public class DeckBuildBattleInfo : ScriptableObject
{
    [ShowInInspector] public string key => name.TrimPrefix($"{nameof(DeckBuildBattleInfo)}_");

    [ValueDropdown(nameof(GetUnitList))]
    public List<UnitInfo> mate;
    public List<Vector2> mateV2 = new() { Vector2.one, Vector2.one, Vector2.one, Vector2.one };
    [ValueDropdown(nameof(GetUnitList))]
    public List<UnitInfo> enemys;
    public List<Vector2> enemyV2 = new() { Vector2.one, Vector2.one, Vector2.one, Vector2.one };


    public Sprite backGround;

    public static DeckBuildBattleInfo Get(string key) => GameAsset.GetInstanceSo<DeckBuildBattleInfo>(key);
    public static DeckBuildBattleInfo Get(DeckBuildBattleData data) => Get(data.key);
    public List<IInfo> GetUnitList()
    {
        return InfoExtension.GetKeyList<UnitInfo>();
    }
}
