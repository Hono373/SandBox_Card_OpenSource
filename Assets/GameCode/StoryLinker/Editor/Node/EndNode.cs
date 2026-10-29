using UnityEditor.Experimental.GraphView;

/// <summary>
/// 结束节点：故事流程的终点
/// </summary>
public class EndNode : InfoNode
{
    public EndNode Init(EndInfo info)
    {
        Set(info);
        InPort(info);
        Expand(info);
        return this;
    }
}
