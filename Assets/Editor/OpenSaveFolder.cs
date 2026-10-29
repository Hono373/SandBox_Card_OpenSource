using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Like.EditorCommon.Assets.Like.EditorCommon.Editor
{
    public class OpenSaveFolder
    {
        [MenuItem("我的工具/打开保存文件夹")]
        public static void OpenPresidentFolder()
        {
            // 这里假设 president 文件夹的路径
            string presidentFolderPath = Path.Combine(Application.persistentDataPath);

            // 检查文件夹是否存在
            if (Directory.Exists(presidentFolderPath))
            {
                // 打开文件夹
                Process.Start(presidentFolderPath);
            }
            else
            {
                // 若文件夹不存在，给出提示
                EditorUtility.DisplayDialog("错误", "Save 文件夹不存在", "确定");
            }
        }

    }
}