using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
public class CreatorTool : Toolbar
{
    public TextField assetName = null;
    ToolbarButton FastTest = null;
    Action Test = null;
    RectangleSelector selector = null;

    bool selectorBool;
    ToolbarButton selectorManager;

    public CreatorTool(StoryLinker creatorWnd)
    {
        var creator = EditorWindow.GetWindow<StoryLinker>();
        AddButtonNamedCreateAsset();

        CreateFlex();

        AddAssetNameField();

        CreateFlex();

        Add(FastTest = new ToolbarButton(() => Test?.Invoke()) { text = "Save" });
        FastTest.style.backgroundColor = Color.red * 0.5f;

        selector = new RectangleSelector();

        selectorManager = new ToolbarButton(() =>
        {
            selectorBool = !selectorBool;
            if (selectorBool)
            {
                selectorManager.text = "关闭框选";

                creator.nodeView.AddManipulator(selector);
            }
            else
            {
                selectorManager.text = "开启框选";
                creator.nodeView.RemoveManipulator(selector);
            }
        })
        { text = "开启框选" };
        selectorBool = false;

        Add(selectorManager);

        Add(new ToolbarButton(() => AssetWnd.ShowWindow(creator)) { text = "资源列表" });

        Add(new ToolbarButton(() => creator.nodeView.viewTransform.position = new(0, 0)) { text = "复位视图" });

        Test += Save;
    }

    private void Save()
    {
        EditorWindow.GetWindow<StoryLinker>().nodeView.Info.Save();
    }

    private void AddButtonNamedCreateAsset()
    {
        Add(new ToolbarButton(() => CreateNewAsset()) { text = "新建视图", style = { backgroundColor = Color.green * 0.5f } });
    }

    public void CreateNewAsset()
    {
        if (EditorWindow.GetWindow<StoryLinker>().nodeView.Info == null)
        {
            PopupWnd.ShowWindow();
            return;
        }
        bool result = EditorUtility.DisplayDialog("警告", $"您确定要新建视图吗？这会丢掉当前的资源视图\n\nTip:创建后的资源会自动保存修改内容\nTip:未保存的资源会丢失，请及时保存资源", "确认", "取消");
        if (result)
        {
            PopupWnd.ShowWindow();
        }
    }

    private void AddAssetNameField()
    {
        assetName = new TextField();
        assetName.label = "资源名";
        assetName.labelElement.style.marginTop = -1;
        assetName.style.width = 240;
        assetName.labelElement.style.marginRight = -100;
        assetName.SetEnabled(false);
        Add(assetName);
    }

    private void CreateFlex()
    {
        var flex = new VisualElement();
        flex.style.flexGrow = 1;
        Add(flex);
    }
}
