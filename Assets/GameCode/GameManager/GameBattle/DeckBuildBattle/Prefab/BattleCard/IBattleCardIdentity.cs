public interface IBattleCardIdentity
{
    BattleCard Card { get; }
    void ForceRefresh();
    string Owner();
    string Key();
    CardInfo Info();
}
