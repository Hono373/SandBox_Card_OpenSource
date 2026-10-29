/// <summary>
/// 选项节点：提供分支选择
/// </summary>
public class OptionNode : InfoNode
{
    public OptionNode Init(OptionInfo info)
    {
        Set(info);
        var inPort = InPort(info);
        var outPort = OutPort(info);
        inPort.portType = outPort.portType = typeof(OptionInfo);
        Expand(info);
        return this;
    }
}
