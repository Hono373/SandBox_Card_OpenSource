using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(fileName = nameof(StoryLinkerConfig), menuName = nameof(StoryLinkerConfig))]
public class StoryLinkerConfig : ScriptableObject
{
    private static StoryLinkerConfig _instance;
    public static StoryLinkerConfig Get()
    {

        if (_instance == null)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(StoryLinkerConfig)}");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _instance = AssetDatabase.LoadAssetAtPath<StoryLinkerConfig>(path);
                if (guids.Length > 1)
                {
                    Debug.LogError($"找到多个{nameof(StoryLinkerConfig)}资源，请删除多余的资源");
                }
            }
            else
            {
                // 创建新资源
                _instance = CreateInstance<StoryLinkerConfig>();
                string path = $"Assets/{nameof(StoryLinkerConfig)}.asset";
                AssetDatabase.CreateAsset(_instance, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        return _instance;

    }

    public StyleSheet graphViewBG;
    [Header("引用")]
    [SerializeField] DefaultAsset CreateTo;
    public string GetCreatePath()
    {
        if (CreateTo == null)
        {
            Debug.LogWarning("CreateTo 未设置，使用默认路径 Assets");
            return "Assets";
        }
        return AssetDatabase.GetAssetPath(CreateTo);
    }
    [Header("配置")]
    public bool DebugMode = false;
    public bool ShowFullTextInfo = false;
}