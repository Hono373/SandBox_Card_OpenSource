using System.Collections.Generic;

public class LastActionIndex
{
    public Queue<int> targetIndexList = new();
    public Dictionary<int, int> counter = new();
    public void GetCounter(int index, out int result)
    {
        if (!counter.TryGetValue(index, out result))
            result = 0;
    }
}

