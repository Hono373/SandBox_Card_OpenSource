using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

/// <summary>
/// Json.NET 反序列化容错：字段级解析失败时置空/默认值继续，不阻断整个对象加载。
/// 典型场景：SerializeReference 多态字段的 $type 找不到对应子类（如某字段类名/命名空间变动）、
/// 字段类型不匹配等。设为 Handled 后该字段置 null（引用）或保持默认（值/集合），整行照常入库，
/// 避免"一个内部字段类型名变了就整张卡加载不了"。
/// 注意：JSON 语法级错误（JObject.Parse）不在此处理，仍由外层 try/catch 拦截并整行跳过。
/// </summary>
public static class JsonDeserializeErrorHandler
{
    /// <summary>当前解析行的 key（ThreadStatic，解析一行前设置、结束后清除），用于日志定位到具体条目。</summary>
    [ThreadStatic] static string currentRowKey;

    /// <summary>解析一行前调用：记录该行 key，供 OnError 日志定位。</summary>
    public static void BeginRow(string key) => currentRowKey = key;

    /// <summary>一行解析结束后调用：清除当前 key，避免串到下一行。</summary>
    public static void EndRow() => currentRowKey = null;

    public static void OnError(object sender, ErrorEventArgs e)
    {
        e.ErrorContext.Handled = true;
        string keyTag = string.IsNullOrEmpty(currentRowKey) ? "" : $"[key={currentRowKey}] ";
        UnityEngine.Debug.LogWarning($"{keyTag}[Json] 字段反序列化失败，已容错置空: {e.ErrorContext.Error.Message}");
    }
}