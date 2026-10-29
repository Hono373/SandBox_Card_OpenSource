public class ValueModifier
{
    //基础伤害
    public int value;
    public int precent;
    //最终伤害
    public int endValue;
    public int endPrecent;
    //最终结果
    public int? forceResult;
    public int CalculateModifiedResult(int baseValue)
    {
        var resultValue = (baseValue + value).GreaterThanOrEqual();
        var resultPrecent = (100 + precent).GreaterThanOrEqual();
        var result = (resultValue * resultPrecent) / 100;

        var resultEnd = (result + endValue).GreaterThanOrEqual();
        var resultEndPrecent = (100 + endPrecent).GreaterThanOrEqual();

        var total = (resultEnd * resultEndPrecent) / 100;

        if (forceResult.HasValue)
        {
            return forceResult.Value;
        }

        return total;
    }
}
