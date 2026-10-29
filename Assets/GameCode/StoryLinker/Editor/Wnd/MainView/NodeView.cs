using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// StoryLinker 的主视图，继承自 GraphView
/// </summary>
public class NodeView : GraphView
{
    public StoryLinker creator = null;
    public StoryInfo Info = null;

    public NodeView(StoryLinker creatorWnd)
    {
        creator = creatorWnd;
        SetInterface();
        graphViewChanged += WhenViewChanged;
    }
    public Type GetGuidType(BaseInfo baseInfo)
    {
        var port = GetPortByGuid(baseInfo.inPort.guid);
        if (port?.node == null)
        {
            Debug.LogWarning($"找不到 GUID 为 {baseInfo.inPort.guid} 的端口");
            return null;
        }
        return port.node.GetType();
    }
    /// <summary>
    /// NodeEdgeClear 会清除 ToolNode/MiniMap，建议只清数据节点和边
    /// </summary>
    public void Fresh(StoryInfo info)
    {
        NodeEdgeClear();
        Info = info;
        AddElement(new ToolNode(Info));
        Add(new MiniMap());
        Debug.Log($"Fresh: Info.list 中有 {info.list?.Count ?? -1} 个节点");
        LoadNodeAndEdge(Info);
    }

    /// <summary>
    /// 加载节点和边
    /// 
    /// 在数据加载场景下，LinkPorts 使用 Port.ConnectTo() 是官方 API，
    /// 用于精确恢复已保存的连接，不需要兼容性检查。详见 LinkPorts 注释。
    /// </summary>
    private void LoadNodeAndEdge(StoryInfo info)
    {
        var start = new StartNode(Info.startInfo);
        AddElement(start);
        CreateNode(info.list);
        if (StoryLinker.GetConfig().DebugMode)
        {
            Debug.Log("In DebugMode,Skip LinkPorts");
            return;
        }
        LinkPorts(Info.startInfo);
        foreach (var nodeInfo in info.list)
        {
            LinkPorts(nodeInfo);
        }
    }
    /// <summary>
    /// Port.ConnectTo 是官方 API，数据加载场景不需要兼容性检查。
    /// outPort 或 inPort 为 null 时跳过即可。
    /// </summary>
    private void LinkPorts(BaseInfo info)
    {
        var outPort = GetPortByGuid(info.outPort.guid);
        foreach (var nextGuid in info.outPort.nextGuids)
        {
            var inPort = GetPortByGuid(nextGuid);
            if (outPort != null && inPort != null)
                AddElement(outPort.ConnectTo(inPort));
        }
    }

    /// <summary>
    /// 根据数据类型创建对应的节点
    /// </summary>
    private void CreateNode<T>(List<T> list) where T : BaseInfo
    {
        foreach (var info in list)
        {
            switch (info)
            {
                case DialogueInfo dialogue:
                    AddElement(new DialogueNode().Init(dialogue));
                    break;
                case OptionInfo option:
                    AddElement(new OptionNode().Init(option));
                    break;
                case EndInfo end:
                    AddElement(new EndNode().Init(end));
                    break;
                case NoteInfo note:
                    AddElement(new NoteNode().Init(note));
                    break;
            }
        }
    }

    /// <summary>
    /// 清除所有节点和边。移除前主动释放 InfoNode 的 PropertyTree，
    /// 避免 Odin 属性树依赖 DetachFromPanelEvent 的不可靠时机。
    /// </summary>
    private void NodeEdgeClear()
    {
        // 先复制集合，避免迭代时修改导致异常
        var edgesToRemove = edges.ToList();
        var nodesToRemove = nodes.ToList();
        foreach (var edge in edgesToRemove)
        {
            RemoveElement(edge);
        }
        foreach (var node in nodesToRemove)
        {
            // 主动释放 Odin PropertyTree，防止其依赖 DetachFromPanelEvent 的不确定时机而泄漏。
            // PropertyTree.Dispose() 未调用时，Odin 的析构函数会报 GC 警告。
            if (node is InfoNode infoNode)
                infoNode.DisposePropertyTree();
            RemoveElement(node);
        }
    }
    /// <summary>
    /// 设置 GraphView 的交互界面
    /// 
    /// 1. 缺少 RectangleSelector（框选功能），用户代码在 CreatorTool 中手动添加
    /// 2. 应该添加 ContentZoomer（缩放）的 Manipulator
    /// </summary>
    private void SetInterface()
    {
        style.flexGrow = 1;

        this.AddManipulator(new SelectionDragger());  // 选择拖动器：拖动选择区域
        this.AddManipulator(new ContentDragger()); // 矩形选择器：拖动选择区域

        SetupZoom(0.25f, 1.25f);

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        styleSheets.Add(StoryLinker.GetConfig().graphViewBG);
    }

    /// <summary>
    /// 获取兼容的端口列表（控制哪些端口可以互相连接）
    /// 
    /// 当前实现允许所有不同方向的端口互相连接，没有类型检查
    /// 应该根据 portType 进行更严格的兼容性检查
    /// </summary>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return (from nap in ports.ToList()
                where nap.direction != startPort.direction && nap.node != startPort.node
                select nap).ToList();
    }

    private GraphViewChange WhenViewChanged(GraphViewChange change)
    {
        bool needSave = OnRemove(change);
        needSave |= OnMove(change);
        needSave |= OnCreateEdge(change);

        if (needSave && Info != null)
        {
            Info.Save();
        }
        return change;
    }

    private static bool OnCreateEdge(GraphViewChange change)
    {
        if (change.edgesToCreate == null) return false;
        //edge的output指的是node的output
        foreach (Edge edge in change.edgesToCreate)
        {
            var thisInfo = edge.output.userData as BaseInfo;
            var nextInfo = edge.input.userData as BaseInfo;
            thisInfo.LinkTo(nextInfo.inPort.guid);
        }
        return true;
    }

    private static bool OnMove(GraphViewChange change)
    {
        if (change.movedElements == null) return false;
        foreach (GraphElement ele in change.movedElements)
        {
            if (ele is InfoNode node)
                node.info.UpdatePos(node.Pos);
        }
        return true;
    }

    private bool OnRemove(GraphViewChange change)
    {
        if (change.elementsToRemove == null) return false;
        foreach (GraphElement ele in change.elementsToRemove)
        {
            if (ele is InfoNode node)
            {
                Info.Remove(node.info);
            }
            if (ele is Edge edge)
            {
                var thisInfo = edge.output.userData as BaseInfo;
                var nextInfo = edge.input.userData as BaseInfo;
                thisInfo.DisConnect(nextInfo.inPort.guid);
            }
        }
        return true;
    }
}
