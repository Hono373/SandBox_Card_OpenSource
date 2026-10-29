using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
public class FastCheck
{
    [MenuItem("我的工具/快速测试")]
    public static void FastCheckMethod()
    {

    }
    private static int _minimumCharacterCount = 10; // 最小字符数阈值
    private static int _deletedFileCount = 0;

    [MenuItem("我的工具/Clean Empty Scripts")]
    public static void RunCleaner()
    {
        _deletedFileCount = 0;
        ProcessScripts();
        EditorUtility.DisplayDialog("清理完成", $"共删除了 {_deletedFileCount} 个空脚本文件", "确定");
        AssetDatabase.Refresh();
    }

    private static void ProcessScripts()
    {
        string assetsPath = Application.dataPath;
        ProcessDirectory(assetsPath);
    }

    private static void ProcessDirectory(string targetDirectory)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory, "*.cs");
        foreach (string fileName in fileEntries)
        {
            ProcessFile(fileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            string dirName = Path.GetFileName(subdirectory);
            if (dirName == "Library" || dirName == "Temp" || dirName == "Obj" || dirName == "Logs")
                continue;

            ProcessDirectory(subdirectory);
        }
    }

    private static void ProcessFile(string path)
    {
        try
        {
            string content = File.ReadAllText(path, Encoding.UTF8);
            string trimmedContent = content.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

            if (trimmedContent.Length == 0 || trimmedContent.Length < _minimumCharacterCount)
            {
                string metaFile = path + ".meta";
                if (File.Exists(metaFile))
                    File.Delete(metaFile);

                File.Delete(path);
                _deletedFileCount++;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"处理文件 {path} 时出错: {ex.Message}");
        }
    }
}
