using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// 校验：构造期创建 DOTween 时间轴（字段初始化器 / 表达式体属性 / 构造函数体）。
///
/// 为什么需要这条约束：
/// GameFlow / GameModify 族的实例会被 [SerializeReference] 存进存档
/// （典型：DeckBuildBattlePhase.currentPhase，见 GameSerialize/BattleData/DeckBuildBattlePhase.cs），
/// 读档时 Newtonsoft 按 $type 构造具体类型的实例，C# 字段初始化器会随之执行。
/// 若此时创建 DOTween，会撞 isPlaying 禁区。所以"构造期不得创建 DOTween"是硬约束。
///
/// 为什么用工具而不是注释：
/// 该约束此前只写在 RootGameFlow 的注释里，实际已被漏过（InsertSettlement 的字段名是 seq 不是 flow）。
/// 本校验把约束从"人记得住"变成"编译后自动报错"。
///
/// 例外机制：
/// 若某处经评估决定保留，把它写进同目录的 FlowTweenCheckExceptions.txt（每行一条，
/// 支持 "相对路径:字段名" 或 "相对路径"，# 开头为注释），该处降级为提示不再报 error。
/// </summary>
public static class FlowTweenCreateValidator
{
    /// <summary>DOTween 时间轴类型名：声明为这些类型的字段/属性若在构造期被赋值创建，即为违规。</summary>
    static readonly string[] TweenTypes = { "Sequence", "Tweener", "Tween", "TweenParams" };

    /// <summary>创建 DOTween 时间轴的入口。项目内时间轴应统一经 GameAnim.Flow / GameAnim.AutoPlay。</summary>
    static readonly string[] CreateTokens = { "GameAnim.Flow", "GameAnim.AutoPlay", "DOTween.Sequence", "DOTween.To", "DOTween.TweensById" };

    const string ExceptionsFile = "FlowTweenCheckExceptions.txt";

    [MenuItem("Tools/校验/构造期创建 DOTween 检查")]
    static void ValidateByMenu()
    {
        Validate(true);
    }

    [DidReloadScripts]
    static void ValidateOnReload()
    {
        Validate(false);
    }

    static void Validate(bool verbose)
    {
        var files = CollectCsFiles();
        var exceptions = LoadExceptions();
        var issues = new List<string>();
        var skipped = new List<string>();

        foreach (var file in files)
        {
            var raw = File.ReadAllText(file);
            var depths = BuildDepths(raw, out var stripped);
            var typeLevel = raw.Contains("namespace ") ? 2 : 1;

            CheckFieldInitializer(file, raw, stripped, depths, typeLevel, exceptions, issues, skipped);
            CheckExpressionBodied(file, raw, stripped, depths, typeLevel, exceptions, issues, skipped);
            CheckConstructorBody(file, raw, stripped, depths, typeLevel, exceptions, issues, skipped);
        }

        if (issues.Count == 0)
        {
            if (verbose)
                Debug.Log($"[构造期DOTween检查] 通过：扫描 {files.Count} 个 .cs，未发现构造期创建 DOTween" +
                          (skipped.Count > 0 ? $"；已评估例外 {skipped.Count} 处" : string.Empty));
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"[构造期DOTween检查] 发现 {issues.Count} 处违规。");
        sb.AppendLine("约束：会被序列化的类型（含 [SerializeReference] 持有的相位/流程对象）不得在构造期创建 DOTween，");
        sb.AppendLine("因为读档反序列化会构造实例并执行字段初始化器，此时创建 DOTween 会撞 isPlaying 禁区。");
        sb.AppendLine("修法：字段只声明不初始化，改为在自身 Start 内用 GameAnim.Flow<T>() 创建。");
        sb.AppendLine("如经评估确认可接受，请在 " + ExceptionsFile + " 中登记例外。");
        sb.AppendLine();
        foreach (var issue in issues)
            sb.Append(issue).AppendLine();

        Debug.LogError(sb.ToString());
    }

    // ---------- 检查项 ----------

    static void CheckFieldInitializer(string file, string raw, string stripped, int[] depths, int typeLevel,
        HashSet<string> exceptions, List<string> issues, List<string> skipped)
    {
        var pattern = @"(?:Sequence|Tweener|Tween|TweenParams)\s+\w+\s*=\s*[^;]*;";
        foreach (Match match in Regex.Matches(stripped, pattern))
        {
            if (depths[match.Index] != typeLevel) continue;      // 只查类体层级，方法内局部变量无害
            if (!ContainsCreateToken(match.Value)) continue;
            Report(file, raw, match.Index, "字段初始化器在构造期创建 DOTween", exceptions, issues, skipped);
        }
    }

    static void CheckExpressionBodied(string file, string raw, string stripped, int[] depths, int typeLevel,
        HashSet<string> exceptions, List<string> issues, List<string> skipped)
    {
        var pattern = @"(?:Sequence|Tweener|Tween|TweenParams)\s+\w+\s*=>[^;]*;";
        foreach (Match match in Regex.Matches(stripped, pattern))
        {
            if (depths[match.Index] != typeLevel) continue;
            if (!ContainsCreateToken(match.Value)) continue;
            Report(file, raw, match.Index, "表达式体属性每次访问都会创建 DOTween", exceptions, issues, skipped);
        }
    }

    static void CheckConstructorBody(string file, string raw, string stripped, int[] depths, int typeLevel,
        HashSet<string> exceptions, List<string> issues, List<string> skipped)
    {
        foreach (Match cls in Regex.Matches(stripped, @"\bclass\s+(\w+)"))
        {
            var className = cls.Groups[1].Value;
            var pattern = @"(?<!new\s)\b" + Regex.Escape(className) + @"\s*\([^)]*\)\s*(?::\s*[^{;]+?)?\{";
            foreach (Match ctor in Regex.Matches(stripped, pattern))
            {
                if (depths[ctor.Index] != typeLevel) continue;
                if (IsNewExpression(stripped, ctor.Index)) continue;

                var braceIndex = ctor.Index + ctor.Length - 1;
                var end = FindBlockEnd(stripped, depths, braceIndex);
                if (end < 0) continue;

                var body = stripped.Substring(braceIndex, end - braceIndex);
                var hit = IndexOfAny(body, CreateTokens);
                if (hit < 0) continue;

                Report(file, raw, braceIndex + hit, "构造函数体内创建 DOTween", exceptions, issues, skipped);
            }
        }
    }

    // ---------- 工具 ----------

    static bool ContainsCreateToken(string text)
    {
        return IndexOfAny(text, CreateTokens) >= 0;
    }

    static int IndexOfAny(string text, string[] tokens)
    {
        var result = -1;
        foreach (var token in tokens)
        {
            var index = text.IndexOf(token, System.StringComparison.Ordinal);
            if (index >= 0 && (result < 0 || index < result))
                result = index;
        }
        return result;
    }

    /// <summary>排除对象初始化器写法（new Foo() { ... }），它形如构造函数签名但不是构造函数。</summary>
    static bool IsNewExpression(string stripped, int index)
    {
        var i = index - 1;
        while (i >= 0 && char.IsWhiteSpace(stripped[i])) i--;
        if (i < 1) return false;
        return stripped[i] == 'w' && i >= 2 && stripped[i - 1] == 'e' && stripped[i - 2] == 'n';
    }

    static int FindBlockEnd(string stripped, int[] depths, int braceIndex)
    {
        for (var i = braceIndex + 1; i < stripped.Length; i++)
        {
            if (stripped[i] == '}' && depths[i] < depths[braceIndex])
                return i;
        }
        return -1;
    }

    static void Report(string file, string raw, int index, string reason,
        HashSet<string> exceptions, List<string> issues, List<string> skipped)
    {
        var line = LineOf(raw, index);
        var relative = ToRelative(file);
        var content = GetLine(raw, line).Trim();
        var entry = relative + ":" + line;
        var text = entry + "  " + reason + "\n    " + content + "\n";

        if (exceptions.Contains(entry) || exceptions.Contains(relative))
        {
            skipped.Add(text);
            return;
        }
        issues.Add(text);
    }

    static int LineOf(string text, int index)
    {
        var line = 1;
        var limit = Mathf.Min(index, text.Length);
        for (var i = 0; i < limit; i++)
            if (text[i] == '\n') line++;
        return line;
    }

    static string GetLine(string text, int line)
    {
        var current = 1;
        var start = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (current == line)
            {
                if (text[i] == '\n') return text.Substring(start, i - start);
                if (i == text.Length - 1) return text.Substring(start);
            }
            else if (text[i] == '\n')
            {
                current++;
                start = i + 1;
            }
        }
        return current == line && start <= text.Length ? text.Substring(start) : string.Empty;
    }

    static List<string> CollectCsFiles()
    {
        var projectRoot = Path.GetDirectoryName(Application.dataPath);
        var assets = Path.Combine(projectRoot, "Assets");
        var files = new List<string>();
        if (!Directory.Exists(assets)) return files;

        foreach (var file in Directory.GetFiles(assets, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/Plugins/")) continue;          // 第三方（含 DOTween 本体）不参与
            if (normalized.Contains("/PackageCache/")) continue;
            if (normalized.Contains("/Library/")) continue;
            files.Add(normalized);
        }
        return files;
    }

    static HashSet<string> LoadExceptions()
    {
        var set = new HashSet<string>();
        var path = Path.Combine(Path.GetDirectoryName(Application.dataPath),
            "Assets/GameCode/Editor/" + ExceptionsFile);
        if (!File.Exists(path)) return set;

        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("#")) continue;
            set.Add(line.Replace('\\', '/'));
        }
        return set;
    }

    static string ToRelative(string absolute)
    {
        var root = Path.GetDirectoryName(Application.dataPath).Replace('\\', '/');
        var normalized = absolute.Replace('\\', '/');
        return normalized.StartsWith(root) ? normalized.Substring(root.Length + 1) : normalized;
    }

    /// <summary>
    /// 逐字符扫描：把注释与字符串内容替换为空格（保持长度不变，便于按索引取行号），
    /// 并记录每个字符位置所处的花括号深度。类体成员深度 = 1（本项目无 namespace）。
    /// </summary>
    static int[] BuildDepths(string text, out string stripped)
    {
        var buffer = text.ToCharArray();
        var depths = new int[text.Length];
        var depth = 0;
        var i = 0;

        while (i < text.Length)
        {
            var c = text[i];
            depths[i] = depth;

            // 行注释
            if (c == '/' && i + 1 < text.Length && text[i + 1] == '/')
            {
                while (i < text.Length && text[i] != '\n')
                {
                    depths[i] = depth;
                    buffer[i] = ' ';
                    i++;
                }
                continue;
            }

            // 块注释
            if (c == '/' && i + 1 < text.Length && text[i + 1] == '*')
            {
                while (i < text.Length)
                {
                    if (i + 1 < text.Length && text[i] == '*' && text[i + 1] == '/')
                    {
                        depths[i] = depth;
                        buffer[i] = ' ';
                        depths[i + 1] = depth;
                        buffer[i + 1] = ' ';
                        i += 2;
                        break;
                    }
                    depths[i] = depth;
                    if (text[i] != '\n') buffer[i] = ' ';
                    i++;
                }
                continue;
            }

            // 字符串（普通 / 逐字 / 插值）
            var isString = c == '"'
                           || (c == '@' && i + 1 < text.Length && text[i + 1] == '"')
                           || (c == '$' && i + 1 < text.Length && text[i + 1] == '"')
                           || (c == '$' && i + 2 < text.Length && text[i + 1] == '@' && text[i + 2] == '"');

            if (isString)
            {
                var verbatim = false;
                if (c == '$')
                {
                    buffer[i] = ' ';
                    i++;
                    if (i < text.Length && text[i] == '@')
                    {
                        verbatim = true;
                        buffer[i] = ' ';
                        i++;
                    }
                }
                else if (c == '@')
                {
                    verbatim = true;
                    buffer[i] = ' ';
                    i++;
                }

                if (i < text.Length && text[i] == '"')
                {
                    depths[i] = depth;
                    buffer[i] = ' ';
                    i++;
                }

                while (i < text.Length)
                {
                    var ch = text[i];
                    if (!verbatim && ch == '\\')
                    {
                        depths[i] = depth;
                        buffer[i] = ' ';
                        i++;
                        if (i < text.Length)
                        {
                            depths[i] = depth;
                            buffer[i] = ' ';
                            i++;
                        }
                        continue;
                    }
                    if (ch == '"')
                    {
                        if (verbatim && i + 1 < text.Length && text[i + 1] == '"')
                        {
                            depths[i] = depth;
                            buffer[i] = ' ';
                            i++;
                            depths[i] = depth;
                            buffer[i] = ' ';
                            i++;
                            continue;
                        }
                        depths[i] = depth;
                        buffer[i] = ' ';
                        i++;
                        break;
                    }
                    depths[i] = depth;
                    if (ch != '\n') buffer[i] = ' ';
                    i++;
                }
                continue;
            }

            // 字符字面量
            if (c == '\'')
            {
                buffer[i] = ' ';
                i++;
                while (i < text.Length && text[i] != '\'')
                {
                    depths[i] = depth;
                    if (text[i] == '\\' && i + 1 < text.Length)
                    {
                        buffer[i] = ' ';
                        i++;
                        depths[i] = depth;
                        buffer[i] = ' ';
                        i++;
                        continue;
                    }
                    buffer[i] = ' ';
                    i++;
                }
                if (i < text.Length)
                {
                    depths[i] = depth;
                    buffer[i] = ' ';
                    i++;
                }
                continue;
            }

            if (c == '{')
            {
                depth++;
                depths[i] = depth;
                i++;
                continue;
            }

            if (c == '}')
            {
                depth--;
                if (depth < 0) depth = 0;
                depths[i] = depth;
                i++;
                continue;
            }

            i++;
        }

        stripped = new string(buffer);
        return depths;
    }
}
