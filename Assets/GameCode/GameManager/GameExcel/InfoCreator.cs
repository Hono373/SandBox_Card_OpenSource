using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class InfoCreator<T> : ScriptableObject where T : class
{
    /// <summary>编辑器唯一实例缓存（泛型类静态字段按 T 分份，天然隔离）。</summary>
    static InfoCreator<T> instance;
    public static InfoCreator<T> Get()
    {
#if UNITY_EDITOR
        if (instance != null) return instance;
        // 找派生自 InfoCreator<T> 的具体类型（如 CardInfoCreator）
        var creatorType = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => !t.IsAbstract && t.IsClass && typeof(InfoCreator<T>).IsAssignableFrom(t));
        if (creatorType == null) return null;
        // 按类型找资产（派生类 CreateAssetMenu 创建），取第一个
        var guids = UnityEditor.AssetDatabase.FindAssets($"t:{creatorType.Name}");
        if (guids.Length == 0) return null;
        instance = UnityEditor.AssetDatabase.LoadAssetAtPath<InfoCreator<T>>(
            UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
        return instance;
#else
        return null;
#endif
    }

    [HorizontalGroup("KeyRow")]
    [ValueDropdown(nameof(GetKeyOptions))]
    [OnValueChanged(nameof(LoadInfoForKey))]
    public string key;

    /// <summary>
    /// 当前编辑的条目。**刻意不序列化**（System.NonSerialized）：数据唯一权威源是
    /// Assets/GameDataBase/{tableName}.json，资产里再存一份副本只会造成两份数据打架
    /// （历史 bug：换 key 后残留的旧副本会把 Inspector 里刚编辑的内容顶掉，
    ///  且资产会长期停在被序列化那一刻的快照上）。
    /// [ShowInInspector] 保证 Odin 照样显示与编辑——不序列化的成员对应
    /// SerializationBackend.None，Odin 对它**仍支持多态下拉与 Null**，只是不落盘。
    /// 因为不再序列化，info 就是 rows[key] 的**同一引用**（见 LoadInfoForKey），
    /// Inspector 的修改直接落在缓存上；代价是域重载（重编译 / 进 Play）后需重新「获取info数据」。
    /// </summary>
    [ShowInInspector, System.NonSerialized] public T info;

    public readonly string tableName = typeof(T).Name;
    protected Dictionary<string, T> rows = new();

    /// <summary>
    /// 子进程（python 写回 / dotnet Luban）最长等待时间，超时即终止。
    /// Unity 是**同步**等待子进程的，没有超时的话，子进程一旦卡住（Excel 弹窗等）
    /// 整个编辑器会无限假死。
    /// </summary>
    const int ProcessTimeoutMs = 180000;

    /// <summary>把 rows[key] 取到 info（同一引用），保证 Inspector 里编辑的就是缓存中的最新数据。</summary>
    void LoadInfoForKey()
    {
        EnsureLoaded();
        info = (!string.IsNullOrEmpty(key) && rows.TryGetValue(key, out var v)) ? v : null;
    }

    protected virtual void OnEnable() => LoadInfoForKey();
    static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        // 与 GameDB 一致：兼容 Luban 的短类型名 $type（"CostValue"）
        SerializationBinder = new ShortTypeNameBinder(),
        ContractResolver = new IgnorePropertiesContractResolver(),
        Converters = { new SpriteJsonConverter() },
        Error = JsonDeserializeErrorHandler.OnError,
    };
    [PropertyOrder(-1)]
    [HorizontalGroup("Top")]
    [Button("刷新清单缓存")]
    void LoadRows()
    {
#if UNITY_EDITOR
        WarnIfSourceNewer();
        // 编辑器模式下，先拼路径 然后用编辑器函数读取json文件，反序列化为row 并提供log提示成功与条目数量
        string path = Path.Combine(Application.dataPath, "GameDataBase", $"{tableName}.json");
        if (!File.Exists(path))
        {
            // 配置文件不存在时自动创建空文件（含目录 + AssetDatabase 刷新，供 YooAsset 收集）
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, string.Empty);
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"[InfoCreator<{typeof(T).Name}>] 配置文件不存在，已创建空文件: {path}");
        }
        LoadFromText(File.ReadAllText(path));
#endif
        Debug.Log($"[InfoCreator<{typeof(T).Name}>] {tableName} 加载成功，共 {rows.Count} 条");
        // rows 已重建，把 info 重新对齐（直接取值，不经 EnsureLoaded，避免在空表时递归）
        info = (!string.IsNullOrEmpty(key) && rows.TryGetValue(key, out var cur)) ? cur : null;
    }
    /// <summary>
    /// 把当前缓存同步回源表格（Excel）。
    /// 默认不支持，派生类重写 TryMapRow 后生效。
    /// 链路：内存实例 -> 「列号->值」映射 -> 临时 JSON -> ExcelToJson/sync_to_excel.py（COM 写回 DB.xlsx）。
    /// 只写映射结果里出现的列，未出现的列保持 Excel 原值不动。
    /// </summary>
    /// <summary>把缓存写回源表格 Excel；成功返回 true。由「写入info并同步表格」调用。</summary>
    bool SyncToExcelInternal()
    {
#if UNITY_EDITOR
        EnsureLoaded();
        if (rows.Count == 0)
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 缓存为空，请先点「刷新清单缓存」");
            return false;
        }

        var payload = new List<Dictionary<string, object>>();
        var skipped = new List<string>();
        foreach (var kv in rows)
        {
            if (!TryMapRows(kv.Value, out var cardRows))
            {
                skipped.Add(kv.Key);
                continue;
            }
            var rowList = new List<Dictionary<string, object>>();
            foreach (var r in cardRows)
            {
                var cellMap = new Dictionary<string, object>();
                foreach (var c in r)
                    cellMap[c.Key.ToString()] = c.Value;
                rowList.Add(cellMap);
            }
            payload.Add(new Dictionary<string, object> { ["key"] = kv.Key, ["rows"] = rowList });
        }

        if (skipped.Count > 0)
            Debug.LogWarning($"[InfoCreator<{typeof(T).Name}>] 以下条目结构不受支持，已跳过（请手工改 Excel）：{string.Join(", ", skipped)}");
        if (payload.Count == 0)
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 没有可同步的条目");
            return false;
        }

        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string script = Path.Combine(projectRoot, "ExcelToJson", "sync_to_excel.py");
        if (!File.Exists(script))
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 找不到写回脚本: {script}");
            return false;
        }

        string payloadPath = Path.Combine(Path.GetTempPath(), $"{tableName}_sync.json");
        File.WriteAllText(payloadPath,
            JsonConvert.SerializeObject(payload, Formatting.None),
            new System.Text.UTF8Encoding(false));

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"\"{script}\" \"{payloadPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        var proc = System.Diagnostics.Process.Start(psi);
        // 异步读管道：同步 ReadToEnd 在子进程卡死时会连主线程一起挂住；再配 WaitForExit 超时兜底。
        var stdoutTask = proc.StandardOutput.ReadToEndAsync();
        var stderrTask = proc.StandardError.ReadToEndAsync();
        if (!proc.WaitForExit(ProcessTimeoutMs))
        {
            try { proc.Kill(); } catch { }
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 写回 Excel 超时（{ProcessTimeoutMs / 1000}s），已终止子进程。");
            return false;
        }
        string stdout = stdoutTask.GetAwaiter().GetResult();
        string stderr = stderrTask.GetAwaiter().GetResult();
        int exitCode = proc.ExitCode;

        // sync_to_excel.py 约定 stdout 输出一行纯 ASCII 的 JSON（中文转义为 \uXXXX），
        // 因此这里按 JSON 解析而不是 Contains("ok": true) 字符串匹配 —— 不受编码影响，
        // 也不会因格式微调误判。解析后中文已还原，可直接打进 Console。
        JObject result = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(stdout)) result = JObject.Parse(stdout.Trim());
        }
        catch (Exception ex)
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 写回脚本输出不是合法 JSON: {ex.Message}\n{stdout.Trim()}");
        }

        // 必须同时看退出码：脚本崩在 import（如缺 pywin32）时 stdout 为空白，
        // 只判 JSON 会得到「失败但无任何提示」的静默失败（换机后踩过）。
        bool ok = exitCode == 0 && result?.Value<bool?>("ok") == true;

        if (!ok)
        {
            // 脚本的 error 字段是最准的原因（如「DB.xlsx 正被 Excel 打开」），必须放在第一个显示：
            // 之前这里把「python 未装 pywin32」写在最前面，会把脚本给出的准确诊断盖在误导性猜测下面。
            Debug.LogError(
                $"[InfoCreator<{typeof(T).Name}>] 写回 Excel 失败 (exit={exitCode})。\n"
                + $"原因: {result?["error"]?.ToString() ?? "(脚本未返回 error。常见：DB.xlsx 正被 Excel 打开，需先保存并关闭；或本机 python 未安装 pywin32)"}\n"
                + $"stdout: {stdout.Trim()}\n"
                + $"stderr: {stderr.Trim()}");
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(stderr))
                Debug.LogWarning($"[InfoCreator<{typeof(T).Name}>] 写回脚本 stderr: {stderr.Trim()}");
            Debug.Log(
                $"[InfoCreator<{typeof(T).Name}>] 同步回表格成功: "
                + $"changed={result["changed"]}, resized={result["resized"]}, "
                + $"missing={result["missing"]}, backup={result["backup"]}");
        }

        UnityEditor.AssetDatabase.Refresh();
        return ok;
#else
        Debug.LogError("[InfoCreator] SyncToExcel 仅编辑器可用");
        return false;
#endif
    }

    /// <summary>
    /// 直接调用 Luban 导出（不经过 CreateJsonCN.bat）。
    ///
    /// 三个要点，缺一不可：
    ///  1. 工作目录必须是 ExcelToJson —— luban.conf 的 dataDir 相对 conf 文件解析，
    ///     而 Luban 的 File.Exists 按当前工作目录查找；
    ///  2. 先输出到临时目录，再把产物回拷 —— Luban 会删除 outputDataDir 里非本次产出的文件；
    ///  3. 产物文件名取 tableName（Table.xml 中 CardDB 声明了 output="CardInfo"）。
    /// </summary>
    bool RunLubanExport()
    {
#if UNITY_EDITOR
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string workDir = Path.Combine(projectRoot, "ExcelToJson");
        string lubanDll = Path.Combine(workDir, "Tools", "Luban", "Luban.dll");
        string conf = Path.Combine(workDir, "luban.conf");

        if (!File.Exists(lubanDll))
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 找不到 Luban: {lubanDll}");
            return false;
        }
        if (!File.Exists(conf))
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 找不到配置: {conf}");
            return false;
        }

        string tmpDir = Path.Combine(workDir, "_out_tmp");
        if (Directory.Exists(tmpDir)) Directory.Delete(tmpDir, true);

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{lubanDll}\" -t all -d json --conf \"{conf}\" "
                      + $"--errorFormat json -x outputDataDir=\"{tmpDir}\"",
            WorkingDirectory = workDir,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        int exitCode;
        string stdout, stderr;
        try
        {
            var proc = System.Diagnostics.Process.Start(psi);
            var stdoutTask = proc.StandardOutput.ReadToEndAsync();
            var stderrTask = proc.StandardError.ReadToEndAsync();
            if (!proc.WaitForExit(ProcessTimeoutMs))
            {
                try { proc.Kill(); } catch { }
                Debug.LogError($"[InfoCreator<{typeof(T).Name}>] Luban 导出超时（{ProcessTimeoutMs / 1000}s），已终止子进程。");
                return false;
            }
            stdout = stdoutTask.GetAwaiter().GetResult();
            stderr = stderrTask.GetAwaiter().GetResult();
            exitCode = proc.ExitCode;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 调用 dotnet 失败（是否已加入 PATH？）: {ex.Message}");
            return false;
        }

        string produced = Path.Combine(tmpDir, $"{tableName}.json");
        string target = Path.Combine(Application.dataPath, "GameDataBase", $"{tableName}.json");

        if (exitCode != 0 || !File.Exists(produced))
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 导出失败(exit={exitCode}):\n{stdout}\n{stderr}");
            try { if (Directory.Exists(tmpDir)) Directory.Delete(tmpDir, true); } catch { }
            return false;
        }

        File.Copy(produced, target, true);
        try { if (Directory.Exists(tmpDir)) Directory.Delete(tmpDir, true); } catch { }

        Debug.Log($"[InfoCreator<{typeof(T).Name}>] Luban 导出完成:\n{stdout.Trim()}"
                  + (string.IsNullOrWhiteSpace(stderr) ? "" : $"\n{stderr.Trim()}"));
        UnityEditor.AssetDatabase.Refresh();
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 把实例映射成若干行（每行是「列号 -> 单元格值」）；默认不支持，由派生类重写。
    /// 多行语义对齐 Luban 的 multi_rows（见 CardInfoRowMapper 注释）。
    /// </summary>
    protected virtual bool TryMapRows(T obj, out List<Dictionary<int, object>> rows)
    {
        rows = null;
        return false;
    }

    /// <summary>
    /// 一键：把当前编辑结果写回源表格（Excel），再用 Luban 重新导出 json。
    /// 顺序必须先 Excel 后 bat —— bat 从 Excel 生成 json，这样内存 / Excel / json 三处一致，
    /// 且 json 始终由 Luban 生成（格式统一、经 schema 与数据校验）。
    /// </summary>
    [Button("写入info并同步表格")]
    public void WriteRows()
    {
#if UNITY_EDITOR
        if (info == null)
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] info 为空，无法写入");
            return;
        }
        if (string.IsNullOrEmpty(key) || !rows.ContainsKey(key))
        {
            Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 请先选择 key（当前 = \"{key}\"），以确定要写回哪一条");
            return;
        }

        // info 已不序列化，正常路径下它就是 rows[key] 的**同一引用**，编辑早已落在缓存上，
        // 这行只是兜底：若 Odin 侧把 info 换成了新实例（如"复制 / 粘贴 / 新建"），仍能写回。
        rows[key] = info;

        if (!SyncToExcelInternal()) return;
        if (!RunLubanExport()) return;
        LoadRows();         // 读回 Luban 新产物，保证面板与磁盘一致
        LoadInfoForKey();   // 面板重新对齐到磁盘
#else
        Debug.LogError("[InfoCreator] WriteRows 仅编辑器可用，运行时配置表只读");
#endif
    }
    protected void LoadFromText(string text)
    {
        rows.Clear();
        if (string.IsNullOrEmpty(text)) return;

        var serializer = JsonSerializer.Create(Settings);

        // Luban 导出格式：整个文件是一个 JSON 数组；旧格式为逐行 jsonl
        if (text.TrimStart().StartsWith("["))
        {
            try
            {
                foreach (var token in JArray.Parse(text))
                    AddRow(token as JObject, serializer);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 数组解析失败: {ex.Message}");
            }
            return;
        }

        foreach (var line in text.Split('\n'))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            try
            {
                AddRow(JObject.Parse(trimmed), serializer);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InfoCreator<{typeof(T).Name}>] 行解析失败: {ex.Message}\n{trimmed}");
            }
        }
    }

    void AddRow(JObject jobj, JsonSerializer serializer)
    {
        if (jobj == null) return;
        string rowKey = GetRowKey(jobj);
        if (string.IsNullOrEmpty(rowKey)) return;
        JsonDeserializeErrorHandler.BeginRow(rowKey);
        try
        {
            rows[rowKey] = jobj.ToObject<T>(serializer);
        }
        finally
        {
            JsonDeserializeErrorHandler.EndRow();
        }
    }
    protected virtual string GetRowKey(JObject jobj) =>
        jobj.TryGetValue("key", StringComparison.OrdinalIgnoreCase, out var v) ? v.ToString() : string.Empty;

    /// <summary>源表格相对项目根的路径；派生类重写后启用「数据是否已过期」检查。</summary>
    protected virtual string SourceXlsxRelativePath => null;

    /// <summary>若源表格比导出的 json 新，提示需要重新导出。</summary>
    void WarnIfSourceNewer()
    {
#if UNITY_EDITOR
        var rel = SourceXlsxRelativePath;
        if (string.IsNullOrEmpty(rel)) return;
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string xlsx = Path.Combine(projectRoot, rel);
        string json = Path.Combine(Application.dataPath, "GameDataBase", $"{tableName}.json");
        if (!File.Exists(xlsx) || !File.Exists(json)) return;
        if (File.GetLastWriteTimeUtc(xlsx) > File.GetLastWriteTimeUtc(json))
            Debug.LogWarning($"[InfoCreator<{typeof(T).Name}>] 源表格({rel})比 {tableName}.json 新，数据可能已过期，请先运行 ExcelToJson/CreateJsonCN.bat 再刷新。");
#endif
    }

    /// <summary>确保缓存已加载：rows 为空时重载一次（域重载后首次访问自动填充）。</summary>
    protected void EnsureLoaded()
    {
        if (rows.Count == 0)
            LoadRows();
    }
    /// <summary>
    /// 下拉选项：显示 [key]name，值为 key。
    /// 显示格式与 InfoExtension.GetDropdownList（KeyDescPair.ToString）保持一致。
    /// </summary>
    public ValueDropdownList<string> GetKeyOptions()
    {
        EnsureLoaded();
        var list = new ValueDropdownList<string>();
        if (rows.Count == 0)
        {
            list.Add("-", "");
            return list;
        }
        foreach (var kv in rows)
        {
            string display = GetDisplayName(kv.Value);
            list.Add(string.IsNullOrEmpty(display) ? kv.Key : $"[{kv.Key}]{display}", kv.Key);
        }
        return list;
    }

    /// <summary>卡表清单（key + 显示名），供只读视图之类的展示使用。</summary>
    public List<KeyDescPair> GetKeyDescPairs()
    {
        EnsureLoaded();
        var list = new List<KeyDescPair>();
        foreach (var kv in rows)
            list.Add(new KeyDescPair(kv.Key, GetDisplayName(kv.Value)));
        return list;
    }

    /// <summary>
    /// 下拉里显示在 key 后面的辅助文本（如中文名）。默认不显示。
    /// 派生类重写即可；将来 name 改成多语言 Text 时，也只需改这里。
    /// </summary>
    protected virtual string GetDisplayName(T obj) => null;
    [HorizontalGroup("KeyRow")]
    [Button("获取info数据")]
    public void GetInfo()
    {
        //当缓存不存在，获取缓存
        EnsureLoaded();
        //如果缓存存在，则用key 从字典里获取实例
        //将获取的实例赋值到info，为空则赋值为null
        info = string.IsNullOrEmpty(key) || !rows.TryGetValue(key, out var instance) ? null : instance;
    }
    [HorizontalGroup("KeyRow")]
    [Button("清空info数据")]
    public void Clear() => info = null;
    [PropertyOrder(-1)]
    [HorizontalGroup("Top")]
    [Button("清空清单数据")]
    public void ClearRows() => rows = new();

    [PropertyOrder(-1)]
    [HorizontalGroup("Top")]
    [Button("数据库瘦身")]
    public void Compact()
    {
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "GameDataBase", $"{tableName}.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[InfoCreator<{typeof(T).Name}>] 配置文件不存在: {path}，跳过瘦身");
            return;
        }
        // 先按文件最新状态重载缓存（文件可能被外部写入过新行）
        LoadRows();
        if (rows.Count == 0)
        {
            Debug.LogWarning($"[InfoCreator<{typeof(T).Name}>] {tableName} 无数据行，跳过瘦身");
            return;
        }
        // 备份 .old（覆盖式全量备份，失败不阻断）
        try
        {
            File.Copy(path, path + ".old", overwrite: true);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[InfoCreator<{typeof(T).Name}>] {tableName} .old 备份失败: {ex.Message}");
        }
        // 按内存最新状态重写主文件：每 id 只留最后一行（单行 JSON + 换行）
        var sb = new System.Text.StringBuilder();
        foreach (var kv in rows)
            sb.AppendLine(kv.Value.SerializeJsonlLine());
        File.WriteAllText(path, sb.ToString());
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log($"[InfoCreator<{typeof(T).Name}>] {tableName} 瘦身完成：{rows.Count} 条（.old 已备份）");
#else
        Debug.LogError("[InfoCreator] Compact 仅编辑器可用，运行时配置表只读");
#endif
    }
}
