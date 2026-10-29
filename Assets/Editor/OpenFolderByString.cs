using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Like.EditorCommon.Assets.Like.EditorCommon.Editor
{
    public class OpenFolderByString
    {
        [MenuItem("我的工具/excel转Json")]
        public static void OpenExcelToJson()
        {
            OpenFolder("ExcelToJson");
        }
        [MenuItem("我的工具/Doc")]
        public static void OpenDoc()
        {
            OpenFolder("Doc");
        }
        public static void OpenFolder(string folerName)
        {
            // 获取Assets文件夹的路径
            string assetsPath = Application.dataPath;
            // 获取Assets文件夹的上级目录
            string parentDirectory = Directory.GetParent(assetsPath).FullName;
            // 组合路径以访问A文件夹
            string folderPath = Path.Combine(parentDirectory, folerName);
            // 检查bat文件是否存在

            // 检查文件夹是否存在
            if (Directory.Exists(folderPath))
            {
                // 打开文件夹
                Process.Start(folderPath);
            }
            else
            {
                // 若文件夹不存在，给出提示
                EditorUtility.DisplayDialog("错误", "ExcelToJson 文件夹不存在", "确定");
            }
        }
    }
}