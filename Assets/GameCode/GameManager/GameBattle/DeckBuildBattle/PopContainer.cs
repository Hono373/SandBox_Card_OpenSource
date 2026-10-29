using System.Collections.Generic;
using UnityEngine;
public class PopContainer : MonoBehaviour
{
    public List<MonoBehaviour> pool = new();
    public T Get<T>() where T : MonoBehaviour
    {
        foreach (var item in pool)
        {
            if (item is T t)
            {
                return t;
            }
        }
        var target = GameAsset.Instantiate<T>(transform);
        pool.Add(target);
        return target;
    }

    internal void Init(DeckBuildBattle ddb)
    {

    }
}