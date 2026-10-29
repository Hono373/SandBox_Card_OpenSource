using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// 工具节点：用于创建新的故事节点
/// </summary>
public class ToolNode : Node
{
    StoryInfo info = null;

    /// <summary>
    /// 构造函数
    /// 
    /// 1. 硬编码位置和大小
    /// 2. 使用 extensionContainer 来存放按钮，但 Node 的 extensionContainer 是用于展开内容的
    /// </summary>
    public ToolNode(StoryInfo info)
    {
        this.info = info;

        title = "[+] 创建节点";
        style.width = 120;
        style.left = 5;
        style.top = 200;
        capabilities &= ~Capabilities.Deletable;
        extensionContainer.Add(CreateDialogue());
        extensionContainer.Add(CreateOption());
        extensionContainer.Add(CreateEnd());

        extensionContainer.Add(CreateNote());
        RefreshExpandedState();
    }

    /// <summary>
    /// 创建"对话节点"按钮
    /// </summary>
    public Button CreateDialogue()
    {
        var btn = new Button(() =>
        {
            var pos = new Vector2(layout.xMax + 20, layout.y + 20);
            var newInfo = new DialogueInfo().Init(info, pos);
            var newNode = new DialogueNode().Init(newInfo);
            parent.Add(newNode);
        });
        btn.text = "对话节点";
        return btn;
    }

    /// <summary>
    /// 创建"分支节点"按钮
    /// </summary>
    private Button CreateOption()
    {
        var btn = new Button(() =>
        {
            var pos = new Vector2(layout.xMax + 20, layout.y + 20);
            var newInfo = new OptionInfo().Init(info, pos);
            var newNode = new OptionNode().Init(newInfo);
            parent.Add(newNode);
        });
        btn.text = "分支节点";

        return btn;
    }

    /// <summary>
    /// 创建"结束节点"按钮
    /// </summary>
    private Button CreateEnd()
    {
        var btn = new Button(() =>
        {
            var pos = new Vector2(layout.xMax + 20, layout.y + 20);
            var newInfo = new EndInfo().Init(info, pos);
            var newNode = new EndNode().Init(newInfo);
            parent.Add(newNode);
        });
        btn.text = "结束节点";

        return btn;
    }

    /// <summary>
    /// 创建"Note节点"按钮
    /// </summary>
    private Button CreateNote()
    {
        var btn = new Button(() =>
        {
            var pos = new Vector2(layout.xMax + 20, layout.y + 20);
            var newInfo = new NoteInfo().Init(info, pos);
            var newNode = new NoteNode().Init(newInfo);
            parent.Add(newNode);
        });
        btn.text = "Note节点";
        return btn;
    }
}
