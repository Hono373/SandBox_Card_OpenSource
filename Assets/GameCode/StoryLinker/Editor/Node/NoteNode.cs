/// <summary>
/// 笔记节点：用于添加备注信息，不影响故事流程
/// </summary>
public class NoteNode : InfoNode
{
    public NoteNode Init(NoteInfo info)
    {
        Set(info);
        Expand(info);
        return this;
    }
}