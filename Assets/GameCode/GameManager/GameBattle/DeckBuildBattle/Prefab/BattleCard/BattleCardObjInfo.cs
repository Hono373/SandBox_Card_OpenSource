using UnityEngine;
[CreateAssetMenu(fileName = nameof(BattleCardObjInfo))]
public class BattleCardObjInfo : ScriptableObject
{
    public static BattleCardObjInfo Get()
    {
        return GameAsset.GetSingletonSo<BattleCardObjInfo>();
    }

    public OutlineConfig outlineShader;
    public Material material;

    public Background bg = new();
    public Banner banner = new();
    public Frame frame = new();
}
