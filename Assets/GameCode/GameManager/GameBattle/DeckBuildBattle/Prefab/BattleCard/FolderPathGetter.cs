#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器专用工具：打印 DefaultAsset 指向的文件夹路径。
/// 整个类型都在 UNITY_EDITOR 内，Player 构建时不存在。
/// </summary>
public class FolderPathGetter
{
    public static void PrintFolderPath(DefaultAsset folderAsset)
    {
        if (folderAsset == null)
        {
            Debug.LogWarning("未指定文件夹");
            return;
        }

        string path = AssetDatabase.GetAssetPath(folderAsset);
        if (AssetDatabase.IsValidFolder(path))
        {
            GameDebug.Log($"文件夹路径是: {path}");
        }
        else
        {
            Debug.LogWarning("指定的 DefaultAsset 不是文件夹");
        }
    }
}
#endif
