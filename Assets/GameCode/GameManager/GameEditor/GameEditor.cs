#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
/// <summary>
/// 会被宏移除的EditorAPI
/// </summary>
public static class GameEditor
{
    public static void CreateAsset<T>(T target, string path, string key) where T : UnityEngine.ScriptableObject
    {
#if UNITY_EDITOR
        var fullPath = System.IO.Path.Combine(path, key + ".asset");
        Debug.Log(fullPath);

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.AssetDatabase.CreateAsset(target, fullPath);
#endif
    }
    public static void PingObject(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorGUIUtility.PingObject(obj);
#endif
    }
    public static void SaveAssets<T>(T obj) where T : UnityEngine.ScriptableObject
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssets();
#endif
    }
    public static bool IsPlaying()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            return true;
#endif
        return false;
    }
    public static void CreateAsset(Object asset, string path)
    {
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(asset, path);
#endif
    }
}
