using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public static class JsonExtensions
{
    public static void Serialize(this object obj, string fullName)
    {
        string path = Path.Combine(Application.persistentDataPath, fullName);
        string jsonData = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new IgnorePropertiesContractResolver(),
        });

        File.WriteAllText(path, jsonData);
        Debug.Log($"{fullName} Save Succeed");
    }

    /// <summary>
    /// 序列化为 jsonl 单行 JSON 文本（紧凑格式，无缩进换行）。
    /// 配合 <see cref="TypeNameHandling.Auto"/> 多态标记（$type），
    /// 用于游戏配置 jsonl 数据库：一行一条记录，append 到 .jsonl 文件尾。
    /// </summary>
    public static string SerializeJsonlLine(this object obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new IgnorePropertiesContractResolver(),
            Converters = { new SpriteJsonConverter() },
        });
    }

    public static T LoadInPersistantDataPath<T>(string fullName) where T : class
    {
        string path = Path.Combine(Application.persistentDataPath, fullName);
        return LoadWithFullPath<T>(path);
    }
    static T LoadWithFullPath<T>(string fullPath) where T : class
    {
        try
        {
            string jsonData = File.ReadAllText(fullPath);
            var data = JsonConvert.DeserializeObject<T>(jsonData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new IgnorePropertiesContractResolver(),
            });
            Debug.Log($"{fullPath} Load Succeed");
            return data;
        }
        catch (Exception ex)
        {
            GameDebug.Log($"[JsonSerializer] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            GameDebug.Log($"{fullPath} Load Failed or Missing");
            return null;
        }
    }
}
