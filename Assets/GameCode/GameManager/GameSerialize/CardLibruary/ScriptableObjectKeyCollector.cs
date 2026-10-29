using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class ScriptableObjectKeyCollector
{
    /// <summary>
    /// 获取所有继承自 Type 的 ScriptableObject，并提取它们的 Key 字段
    /// </summary>
    /// <typeparam cardName="Type">ScriptableObject 的基类</typeparam>
    /// <returns>所有 Key 字段的列表</returns>
    public static List<string> CollectKeysFromAssets<T>() where T : ScriptableObject
    {
        List<string> keys = new List<string>();

#if UNITY_EDITOR
        // 查找所有继承自 Type 的 ScriptableObject 资源
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset != null)
            {
                // 使用反射获取 Key 字段（支持 public 和 [SerializeField]）
                var field = typeof(T).GetField("key",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (field != null && field.FieldType == typeof(string))
                {
                    string keyValue = field.GetValue(asset) as string;
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        keys.Add(keyValue);
                    }
                }
                else
                {
                    Debug.LogWarning($"找不到字段 'key' 于 {asset.name}");
                }
            }
        }
#endif

        return keys;
    }
}
