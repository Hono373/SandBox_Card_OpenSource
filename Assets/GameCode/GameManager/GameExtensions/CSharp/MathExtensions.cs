public static class MathExtensions
{
    public static int GreaterThanOrEqual(this int numb, int greaterThan = 0)
    {
        if (numb < greaterThan)
            return greaterThan;
        return numb;
    }
    public static float GreaterThanOrEqual0(this float numb)
    {
        if (numb < 0)
            return 0;
        return numb;
    }
}


