/// <summary>
/// 目前替换没有时机意义，但预留
/// </summary>
public interface IBattleCardName
{
    string BaseName { get; }
    string Name(BattleCard card);
}
