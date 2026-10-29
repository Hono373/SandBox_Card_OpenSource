/// <summary>
/// 对话节点：显示对话内容
/// </summary>
public class DialogueNode : InfoNode
{
    public DialogueNode Init(DialogueInfo info)
    {
        Set(info);
        InPort(info);
        OutPort(info);
        Expand(info);
        return this;
    }
}
