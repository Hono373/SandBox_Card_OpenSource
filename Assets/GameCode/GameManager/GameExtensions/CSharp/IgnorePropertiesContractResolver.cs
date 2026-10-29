using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

/// <summary>
/// 全局 Json.NET 契约解析器：只序列化字段，永远不序列化任何属性。
/// 适用于本项目约定"可序列化数据一律用字段承载"。
/// </summary>
public class IgnorePropertiesContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (member.MemberType == MemberTypes.Property)
        {
            property.Ignored = true;
        }
        return property;
    }
}
