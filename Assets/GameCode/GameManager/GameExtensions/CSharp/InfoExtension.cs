using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
public static class InfoExtension
{


    public static List<IInfo> GetKeyList<T>() where T : ScriptableObject, IInfo
    {
        List<IInfo> keys = new();
#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null && !string.IsNullOrEmpty(asset.Key()))
            {
                keys.Add(asset);
            }
        }
#endif
        return keys;
    }
    public static List<KeyDescPair> GetKeyList<T>(Func<T, KeyDescPair> getInfo) where T : ScriptableObject
    {
        List<KeyDescPair> keys = new();
#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            var info = getInfo(asset);
            if (asset != null && !string.IsNullOrEmpty(info.key))
            {
                keys.Add(info);
            }
        }
#endif
        return keys;
    }

    /// <summary>生成 Odin ValueDropdown 选项列表（显示 "key - name"，值为 key）。</summary>
    public static ValueDropdownList<string> GetDropdownList<T>(Func<T, KeyDescPair> getInfo) where T : ScriptableObject
    {
        var list = new ValueDropdownList<string>();
#if UNITY_EDITOR
        foreach (var info in GetKeyList(getInfo))
        {
            list.Add(info.ToString(), info.key);
        }
#endif
        return list;
    }
}
