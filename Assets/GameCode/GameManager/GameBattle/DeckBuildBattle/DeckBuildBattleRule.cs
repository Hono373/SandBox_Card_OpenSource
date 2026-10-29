using UnityEngine;

[CreateAssetMenu]
public class DeckBuildBattleRule : ScriptableObject
{
    public static DeckBuildBattleRule Get()
    {
        return GameAsset.GetSingletonSo<DeckBuildBattleRule>();
    }

    public int maxCost = 3;

    public int cardLimit = 7;

    public int drawNumb = 5;
}