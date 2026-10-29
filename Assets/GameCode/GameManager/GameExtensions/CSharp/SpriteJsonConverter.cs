using Newtonsoft.Json;
using System;
using UnityEngine;

/// <summary>
/// Sprite 与字符串 key 互转的 JsonConverter（仅配置序列化使用，不挂存档）。
/// 序列化：Sprite → sprite.name（YooAsset 资源名）。
/// 反序列化：key → Sprite（Editor 走 AssetDatabase，Runtime 走 GameAsset.GetSprite）。
/// </summary>
public class SpriteJsonConverter : JsonConverter<Sprite>
{
    public override void WriteJson(JsonWriter writer, Sprite value, JsonSerializer serializer)
    {
        if (value != null)
            writer.WriteValue(value.name);
        else
            writer.WriteNull();
    }

    public override Sprite ReadJson(JsonReader reader, Type objectType, Sprite existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var key = reader.Value as string;
        if (string.IsNullOrEmpty(key)) return null;
#if UNITY_EDITOR
        // 根目录确定（Assets/GameRes/Sprite），但 sprite 可能在任意子目录：全列 + 按文件名精确比对
        var guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/GameRes/Sprite" });
        foreach (var guid in guids)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (string.Equals(System.IO.Path.GetFileNameWithoutExtension(path), key, StringComparison.OrdinalIgnoreCase))
                return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        Debug.LogError($"[SpriteJsonConverter] Sprite 未找到: {key}（Assets/GameRes/Sprite 递归搜索）");
        return null;
#else
        return GameAsset.GetSprite(key);
#endif
    }
}
