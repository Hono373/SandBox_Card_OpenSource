using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[Serializable]
public class GameDB
{
    public static GameDB Get() => GameManager.Instance.gameDB;
    public static GameDB Create()
    {
        var instance = new GameDB();
        instance.Init();
        return instance;
    }
    public Dictionary<string, CardInfo> cardInfos = new();
    public Dictionary<string, UnitInfo> unitInfos = new();
    public Dictionary<string, ModifierInfo> modifierInfos = new();

    /// <summary>反序列化设置：$type 多态还原 + 只认字段 + Sprite 自动转 key（与 InfoCreator 一致）。</summary>
    static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        // Luban 导出的 $type 只有短类型名（"CostValue"），Newtonsoft 默认要求
        // "CostValue, Assembly-CSharp"；用 Binder 兼容两种写法。
        SerializationBinder = new ShortTypeNameBinder(),
        ContractResolver = new IgnorePropertiesContractResolver(),
        Converters = { new SpriteJsonConverter() },
        Error = JsonDeserializeErrorHandler.OnError,
    };

    private void Init()
    {
        // 通过GameAsset获取 ，赋值到 对应字典
        cardInfos = LoadInfo<CardInfo>();
        // 未来 Info 接入：unitInfos = LoadInfo<UnitInfo>(); modifierInfos = LoadInfo<ModifierInfo>();
    }

    /// <summary>
    /// 泛型加载配置表：类型名即文件名（Assets/GameDataBase/{typeof(T).Name}.json / YooAsset location）。
    /// </summary>
    Dictionary<string, T> LoadInfo<T>(string tableName = null) where T : class
    {
        tableName ??= typeof(T).Name;
        var result = new Dictionary<string, T>();
        var handle = GameAsset.package.LoadAssetSync<TextAsset>(tableName);
        if (handle.AssetObject is TextAsset ta)
            LoadInfoFromText(ta.text, result);
        else
            Debug.LogError($"[GameDB] {tableName} 配置加载失败，请检查 YooAsset 收集规则（Assets/GameDataBase）");
        return result;
    }

    /// <summary>
    /// 核心解析，两种输入都支持：
    ///   - Luban 导出格式：整个文件是一个 JSON 数组（首字符 '['）
    ///   - 旧 jsonl 格式：每行一条（保留兼容）
    /// 同 key 后写覆盖；key 取行内 "key" 字段（忽略大小写）。
    /// </summary>
    void LoadInfoFromText<T>(string text, Dictionary<string, T> result) where T : class
    {
        result.Clear();
        if (string.IsNullOrEmpty(text)) return;

        var serializer = JsonSerializer.Create(Settings);

        if (text.TrimStart().StartsWith("["))
        {
            try
            {
                foreach (var token in JArray.Parse(text))
                    AddRecord(token, result, serializer);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameDB] {typeof(T).Name} 数组解析失败: {ex.Message}");
            }
        }
        else
        {
            foreach (var line in text.Split('\n'))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                try
                {
                    AddRecord(JObject.Parse(trimmed), result, serializer);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GameDB] {typeof(T).Name} 行解析失败: {ex.Message}\n{trimmed}");
                }
            }
        }
        Debug.Log($"[GameDB] {typeof(T).Name} 加载成功，共 {result.Count} 条");
    }

    /// <summary>单条记录入字典：取 key 字段，同 key 后写覆盖。</summary>
    void AddRecord<T>(JToken token, Dictionary<string, T> result, JsonSerializer serializer) where T : class
    {
        var jobj = token as JObject;
        if (jobj == null) return;
        string key = jobj.TryGetValue("key", StringComparison.OrdinalIgnoreCase, out var v) ? v.ToString() : null;
        if (string.IsNullOrEmpty(key)) return;
        JsonDeserializeErrorHandler.BeginRow(key);
        try
        {
            result[key] = jobj.ToObject<T>(serializer);
        }
        finally
        {
            JsonDeserializeErrorHandler.EndRow();
        }
    }
}
