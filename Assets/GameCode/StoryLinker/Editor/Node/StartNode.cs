using UnityEditor.Experimental.GraphView;
/// <summary>
/// 开始节点：故事流程的起点
/// </summary>
public class StartNode : InfoNode
{
    public StartNode(StartInfo info)
    {
        capabilities &= ~Capabilities.Deletable;
        Set(info);
        OutPort(info);
        Expand(info);
    }
}