using UnityEngine;

public static class GameCalculate
{
    public static float Cut(this float width, int numb)
    {
        if (numb <= 0)
        {
            GameDebug.Log("Cut <= 0");
            return 0;
        }
        return width / numb;
    }
}
