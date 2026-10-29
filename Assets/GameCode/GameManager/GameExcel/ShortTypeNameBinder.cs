using System;
using Newtonsoft.Json.Serialization;

/// <summary>
/// 让 Newtonsoft 的 TypeNameHandling.Auto 同时接受两种 $type 写法：
///   - 完整形式 "CostValue, Assembly-CSharp"（Newtonsoft 自己序列化时写的）
///   - 短形式   "CostValue"                        （Luban 导出时写的）
/// 先按原样交给默认 Binder；失败则取逗号前的类型短名，在已加载程序集里查找。
/// </summary>
public class ShortTypeNameBinder : DefaultSerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        try
        {
            var resolved = base.BindToType(assemblyName, typeName);
            if (resolved != null) return resolved;
        }
        catch (Exception)
        {
            // 短类型名会让默认 Binder 抛异常，落下去按短名查找
        }

        string shortName = typeName;
        int comma = typeName.IndexOf(',');
        if (comma >= 0)
            shortName = typeName.Substring(0, comma).Trim();

        // 先查当前程序集与入口程序集，再兜底遍历（顺序影响性能，命中即返回）
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = asm.GetType(shortName, false);
            if (t != null) return t;
        }
        return null;
    }
}
