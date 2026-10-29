using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

/// <summary>
/// 一键打包：先构建 YooAsset 资源包（Builtin 管线，输出到 StreamingAssets/yoo），再构建 Windows Player。
/// 命令行：
///   Unity.exe -batchmode -projectPath &lt;项目&gt; -executeMethod BuildScript.BuildWindows -logFile &lt;日志&gt;
/// 菜单：Build/一键打包 Windows
/// </summary>
public static class BuildScript
{
    private const string PackageName = "YooAsset";
    private const string PackageVersion = "1.0.0";
    private const string OutputDir = @"C:\Users\Fanphic\Desktop\build";
    private const string ExeName = "CardSandBox.exe";
    private const string MainScene = "Assets/Scenes/Game.unity";

    /// <summary>项目根目录（Assets 的父级）</summary>
    private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;

    /// <summary>YooAsset 资源包构建缓存目录（项目内，不进包）</summary>
    private static string BundleOutputRoot => Path.Combine(ProjectRoot, "BuildCache", "Bundles");

    /// <summary>内置资源根目录：运行时 OfflinePlayMode 读 StreamingAssets/yoo/{PackageName}</summary>
    private static string BuildinFileRoot => Path.Combine(Application.dataPath, "StreamingAssets", "yoo");

    [MenuItem("Build/一键打包 Windows（YooAsset + Player）")]
    public static void BuildWindowsFromMenu()
    {
        bool ok = RunBuild();
        EditorUtility.DisplayDialog("打包结果", ok ? "成功" : "失败，详见 Console", "OK");
    }

    /// <summary>命令行入口：自己控制退出码，不依赖 -quit</summary>
    public static void BuildWindows()
    {
        bool ok = false;
        try
        {
            ok = RunBuild();
        }
        catch (Exception e)
        {
            Debug.LogError($"[BuildScript] 异常：{e}");
            ok = false;
        }
        Debug.Log($"[BuildScript] 结束，结果={(ok ? "SUCCESS" : "FAIL")}");
        EditorApplication.Exit(ok ? 0 : 1);
    }

    /// <summary>
    /// 供 Editor 内的自动构建钩子调用：跑完整流程但**不退出 Editor**。
    /// BuildWindows() 结尾会 EditorApplication.Exit()，那是给命令行用的，
    /// 在已打开的 Editor 里调用会把用户正在编辑的工程一起关掉。
    /// </summary>
    public static bool RunBuildNoExit()
    {
        return RunBuild();
    }

    private static bool RunBuild()
    {
        if (!SwitchPlatform()) return false;
        if (!BuildBundles()) return false;
        return BuildPlayer();
    }

    private static bool SwitchPlatform()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
            return true;

        Debug.Log($"[BuildScript] 切换平台 {EditorUserBuildSettings.activeBuildTarget} → StandaloneWindows64");
        bool ok = EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        if (!ok) Debug.LogError("[BuildScript] 切换构建平台失败");
        return ok;
    }

    /// <summary>构建 YooAsset 资源包，并拷贝到 StreamingAssets/yoo/YooAsset</summary>
    private static bool BuildBundles()
    {
        var parameters = new BuiltinBuildParameters
        {
            BuildOutputRoot = BundleOutputRoot,
            BuildinFileRoot = BuildinFileRoot,
            BuildPipeline = "BuiltinBuildPipeline",
            BuildTarget = BuildTarget.StandaloneWindows64,
            BuildMode = EBuildMode.ForceRebuild,
            PackageName = PackageName,
            PackageVersion = PackageVersion,
            CompressOption = ECompressOption.LZ4,
            FileNameStyle = EFileNameStyle.HashName,
            VerifyBuildingResult = true,
            EnableSharePackRule = true,
            BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll,
            BuildinFileCopyParams = string.Empty,
        };

        var pipeline = new BuiltinBuildPipeline();
        var result = pipeline.Run(parameters, true);
        if (!result.Success)
        {
            Debug.LogError($"[BuildScript] YooAsset 构建失败 task={result.FailedTask} error={result.ErrorInfo}");
            return false;
        }
        Debug.Log($"[BuildScript] YooAsset 构建成功 → {result.OutputPackageDirectory}");
        return true;
    }

    private static bool BuildPlayer()
    {
        Directory.CreateDirectory(OutputDir);

        var options = new BuildPlayerOptions
        {
            scenes = new[] { MainScene },
            locationPathName = Path.Combine(OutputDir, ExeName),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        };

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;
        Debug.Log($"[BuildScript] Player 结果={summary.result} " +
                  $"错误={summary.totalErrors} 警告={summary.totalWarnings} " +
                  $"大小={summary.totalSize / 1024f / 1024f:F1}MB 耗时={summary.totalTime}");
        return summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded;
    }
}
