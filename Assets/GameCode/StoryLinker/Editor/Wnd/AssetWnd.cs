using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetWnd : EditorWindow
{
    static StoryLinker creator;
    ListView AssetList;

    // 缓存数据源（防止GC，同时保持引用）
    private List<StoryInfo> cachedAssets = new List<StoryInfo>();
    private const int ITEM_HEIGHT = 20;  // 固定行高，提升性能

    public static void ShowWindow(StoryLinker wnd)
    {
        creator = wnd;
        var window = GetWindow<AssetWnd>("资源列表");
        window.minSize = new Vector2(240, 200);
    }

    public void CreateGUI()
    {
        titleContent = new GUIContent("资源列表[StoryData]",
            EditorGUIUtility.IconContent("d_Folder Icon").image as Texture2D);

        // 初始化数据
        RefreshAssetData();

        // 创建ListView（只创建一次，复用）
        AssetList = CreateListView();
        rootVisualElement.Add(AssetList);
    }

    /// <summary>
    /// 创建ListView（只调用一次）
    /// </summary>
    private ListView CreateListView()
    {
        var listView = new ListView(cachedAssets, ITEM_HEIGHT, MakeItem, BindItem);
        listView.style.flexGrow = 1;

        // 改回 selectionChanged，保留单击弹窗逻辑
        listView.selectionChanged += OnSelectionChanged;

        return listView;
    }

    /// <summary>
    /// 创建列表项UI
    /// </summary>
    private VisualElement MakeItem()
    {
        return new Label();
    }

    /// <summary>
    /// 绑定数据到UI
    /// </summary>
    private void BindItem(VisualElement element, int index)
    {
        var label = element as Label;
        if (label != null && index < cachedAssets.Count && cachedAssets[index] != null)
        {
            label.text = cachedAssets[index].name;
        }
        else
        {
            label.text = "无效资源";
        }
    }

    /// <summary>
    /// 刷新数据源（不重建ListView）
    /// </summary>
    private void RefreshAssetData()
    {
        // 记录当前选中的资源
        StoryInfo selectedAsset = null;
        if (AssetList != null && AssetList.selectedItem != null)
        {
            selectedAsset = AssetList.selectedItem as StoryInfo;
        }

        // 重新加载所有资源
        var newAssets = LoadAllStoryInfos();

        // 更新缓存（保持引用，不重新分配列表）
        cachedAssets.Clear();
        cachedAssets.AddRange(newAssets);

        // 通知ListView数据已改变
        if (AssetList != null)
        {
            AssetList.RefreshItems();  // 刷新可见项

            // 恢复选中状态（如果资源还存在）
            if (selectedAsset != null)
            {
                int newIndex = cachedAssets.FindIndex(asset => asset == selectedAsset);
                if (newIndex >= 0)
                {
                    AssetList.SetSelection(newIndex);
                }
                else
                {
                    AssetList.ClearSelection();
                }
            }
        }
    }

    /// <summary>
    /// 加载所有StoryInfo资源
    /// </summary>
    private List<StoryInfo> LoadAllStoryInfos()
    {
        var guids = AssetDatabase.FindAssets("t:StoryInfo", new[] { StoryLinkerConfig.Get().GetCreatePath() });
        return guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(path => AssetDatabase.LoadAssetAtPath<StoryInfo>(path))
            .Where(asset => asset != null)
            .ToList();
    }

    /// <summary>
    /// 单击选择时触发弹窗
    /// </summary>
    private void OnSelectionChanged(IEnumerable<object> enumerable)
    {
        var storyData = enumerable?.FirstOrDefault() as StoryInfo;
        if (storyData == null) return;

        bool result = EditorUtility.DisplayDialog("读取确认",
            $"您确定要读取{storyData.name}吗？ \n (当前的视图会被重建，但做出的修改已经被保存)",
            "是", "否");

        if (result)
        {
            GetWindow<StoryLinker>().LoadData(storyData);
        }

        // 清除选中状态，否则同一个项无法再次触发 selectionChanged
        AssetList.ClearSelection();
    }

    /// <summary>
    /// 项目资源变更时自动刷新（不重建ListView）
    /// </summary>
    private void OnProjectChange()
    {
        if (AssetList != null)
        {
            RefreshAssetData();
        }
    }

    /// <summary>
    /// 窗口获得焦点时可选择性刷新
    /// </summary>
    private void OnFocus()
    {
        // 可选：失焦后再聚焦时刷新，确保数据最新
        // RefreshAssetData();
    }
}