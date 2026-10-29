using System;
[Serializable]
public class SortedWeight<T>
{
    private int maxWeight = 0;
    private T maxObject = default!;
    public bool HasValue { get; private set; } = false;
    public void Add(int weight, T obj)
    {
        if (weight >= maxWeight)
        {
            maxWeight = weight;
            maxObject = obj;
            HasValue = true;
        }
    }
    public void Clear()
    {
        maxWeight = 0;
        maxObject = default!;
        HasValue = false;
    }
    public T Max() => maxObject;
    public T MaxOrDefault(Func<T> @default) => HasValue ? maxObject : @default.Invoke();
    public T MaxOrDefault(T @default) => HasValue ? maxObject : @default;
}

