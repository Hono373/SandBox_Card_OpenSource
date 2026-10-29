public class BattleCardCalculateContext
{
    public BattleCard Card { protected set; get; }
    public int ValueDefault { protected set; get; }
    public int Addition;
    public int Multiplication;
    public SortedWeight<int> Always { protected set; get; } = new();
    public virtual int Result()
    {
        if (Always.HasValue)
        {
            return Always.Max();
        }
        var result = ((ValueDefault + Addition).GreaterThanOrEqual() * (100 + Multiplication).GreaterThanOrEqual()) / 100;
        return result;
    }
}
