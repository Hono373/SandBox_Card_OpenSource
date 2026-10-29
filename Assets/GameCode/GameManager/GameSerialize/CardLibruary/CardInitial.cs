using Sirenix.OdinInspector;
using System;

/// <summary>
/// 牌组里的一项：一张卡 + 数量。
///
/// 只保存 key（不再保存 KeyDescPair）—— 中文名由卡表实时提供，
/// 避免存档里残留旧名字，也避免 CardInitial 与 GameInfo 互相引用（循环序列化）。
/// 下拉选项直接取自卡表数据（InfoCreator&lt;CardInfo&gt; 的缓存），显示为 [key]中文名。
/// </summary>
[Serializable]
public class CardInitial
{
    [ValueDropdown(nameof(GetCardOptions))]
    public string key;
    [MinValue(0)]
    public int numb = 0;

    /// <summary>兼容旧调用点的只读访问。</summary>
    public string Key => key;

    ValueDropdownList<string> GetCardOptions()
    {
        var creator = InfoCreator<CardInfo>.Get();
        return creator != null ? creator.GetKeyOptions() : new ValueDropdownList<string>();
    }
}
