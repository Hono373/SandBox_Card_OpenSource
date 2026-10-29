using System.Collections.Generic;
using UnityEngine;

public static class GameDebug
{
    public static bool isActive;
    static List<string> buffer = new();
    const int BUFFER_MAX = 500;

    /// <summary>
    /// 2026-10-23 接上开关：GameManager 启动时本来就调了这个方法，但它一直是空方法，
    /// 于是打包版 isActive 恒为 false —— 所有 GameDebug.Log 被静默丢弃，Player.log 里
    /// 连一条意图判定都看不到，查结算/意图问题只能靠读代码推。
    /// DebugConfig 资源（Assets/GameRes/Info/DebugConfig）本来就在，只是一直没人读它。
    /// 注意：GetSo 取不到时会现场 new 一个默认实例（其 isActive 字段默认 true），
    /// 所以资源丢了也不会把日志整个关掉。
    /// </summary>
    public static void Create()
    {
        isActive = GameAsset.GetSo<DebugConfig>().isActive;
    }
    public static void Activate() => isActive = true;

    public static void Log(object message)
    {
#if UNITY_EDITOR
        var line = $"[{Time.realtimeSinceStartup:F2}] {message}";
        Debug.Log(line);
        buffer.Add(line);
        if (buffer.Count > BUFFER_MAX)
            buffer.RemoveRange(0, buffer.Count - BUFFER_MAX);
#else
        if (isActive)
        {
            var line = $"[{Time.realtimeSinceStartup:F2}] {message}";
            Debug.Log(line);
            buffer.Add(line);
            if (buffer.Count > BUFFER_MAX)
                buffer.RemoveRange(0, buffer.Count - BUFFER_MAX);
        }
#endif
    }

    public static string ExportLog() => string.Join("\n", buffer);
    public static void Clear() => buffer.Clear();

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, object message)
        => Debug.Assert(condition, message);
}
