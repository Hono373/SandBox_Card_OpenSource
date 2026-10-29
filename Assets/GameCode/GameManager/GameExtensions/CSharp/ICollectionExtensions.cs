using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于创建7位Id
/// </summary>
public static class ICollectionExtensions
{
    public static string RandomID(this ICollection<string> keyHash)
    {
        var id = CreateID();
        while (keyHash.Contains(id))
        {
            id = CreateID();
        }
        return id;
    }
    static string CreateID()
    {
        return Random.Range(1000000, 10000000).ToString();
    }
    static string CreateID(int value)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[value];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(stringChars);
    }
}
