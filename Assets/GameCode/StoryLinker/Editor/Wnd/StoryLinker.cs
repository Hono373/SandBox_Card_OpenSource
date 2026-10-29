using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.EditorGUIUtility;
/// <summary>
/// StoryLinker 主窗口
///
/// </summary>
public class StoryLinker : EditorWindow
{
    [MenuItem("我的工具/StoryLinker.Test")]
    public static void ShowExample()
    {
        // 吸附到 SceneView 标签栏（作为同区的标签页）
        GetWindow<StoryLinker>("StoryLinker", true, typeof(SceneView));
    }

    public CreatorTool ToolBar = null;
    public NodeView nodeView = null;

    public void CreateGUI()
    {
        titleContent = new GUIContent("StoryLinker.Test", IconContent("d__Popup@2x").image as Texture2D);
        minSize = new Vector2(800, 600);

        var root = rootVisualElement;
        root.Add(ToolBar ??= new(this));

        var viewContainer = new VisualElement();

        viewContainer.style.flexGrow = 1;
        viewContainer.Add(nodeView ??= new NodeView(this));
        root.Add(viewContainer);
    }

    public void CreateAsset(string received)
    {
        var info = CreateInstance<StoryInfo>();
        string assetPath = AssetCreatePath($"{received}.asset");
        AssetDatabase.CreateAsset(info, assetPath);
        info.Save();
        LoadData(info);
    }

    public void LoadData(StoryInfo info)
    {
        ToolBar.assetName.value = info.name;
        nodeView.viewTransform.position = new(0, 0);
        nodeView.Fresh(info);
    }

    public override void DiscardChanges()
    {
        base.DiscardChanges();
    }

    public override void SaveChanges()
    {
        base.SaveChanges();
    }

    void OnFocus() { }
    private void OnLostFocus() { }

    private void OnDestroy()
    {
        // 安全保存，避免 null 引用
        if (nodeView != null && nodeView.Info != null)
        {
            nodeView.Info.Save();
        }
    }

    public string AssetCreatePath(string key) => Path.Combine(GetConfig().GetCreatePath(), key);

    public static StoryLinkerConfig GetConfig() => StoryLinkerConfig.Get();
}
