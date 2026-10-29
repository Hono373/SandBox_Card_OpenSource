#if !ODIN_INSPECTOR
// 2026-10-30 开源准备：项目中若干字段/方法标注了 Odin Inspector 的特性。
// Odin 是商业插件，不能随仓库分发，因此这里提供一组同名的空特性与工具类型：
//   - 装了 Odin 时 ODIN_INSPECTOR 已定义，本文件整体被预处理排除，不影响原有效果；
//   - 没装 Odin 时由本文件补上类型，代码可正常编译运行，只是 Inspector 失去分组/按钮等美化。
// 每个成员的签名与 Odin 真实 API 保持一致，新增 Odin 用法时需同步在此补充。

using System;
using System.Collections.Generic;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class OdinShimAttribute : Attribute
    {
    }

    /// <summary>数值下限提示。Odin 签名：MinValue(double) / MinValue(double, string expression)。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class MinValueAttribute : OdinShimAttribute
    {
        public MinValueAttribute(double minValue) { }
        public MinValueAttribute(double minValue, string expression) { }
    }

    /// <summary>让非序列化成员也显示在 Inspector 里。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class ShowInInspectorAttribute : OdinShimAttribute
    {
    }

    /// <summary>Inspector 中只读。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class ReadOnlyAttribute : OdinShimAttribute
    {
    }

    /// <summary>字段排序。Odin 用 double 而非 int。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class PropertyOrderAttribute : OdinShimAttribute
    {
        public PropertyOrderAttribute(double order) { }
    }

    /// <summary>横向并排分组。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class HorizontalGroupAttribute : OdinShimAttribute
    {
        public HorizontalGroupAttribute(string group) { }
    }

    /// <summary>方法显示为按钮。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class ButtonAttribute : OdinShimAttribute
    {
        public ButtonAttribute() { }
        public ButtonAttribute(string name) { }
    }

    /// <summary>只在该成员所在对象处于运行时隐藏。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class HideInPlayModeAttribute : OdinShimAttribute
    {
    }

    /// <summary>只在该成员所在对象处于编辑器（非运行）时隐藏。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class HideInEditorModeAttribute : OdinShimAttribute
    {
    }

    /// <summary>下拉选择，参数为提供候选值的成员名。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class ValueDropdownAttribute : OdinShimAttribute
    {
        public ValueDropdownAttribute(string memberName) { }
    }

    /// <summary>对多态列表做类型过滤，参数为返回候选类型的成员名。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class TypeFilterAttribute : OdinShimAttribute
    {
        public TypeFilterAttribute(string memberName) { }
    }

    /// <summary>列表绘制设置，命名参数形式使用。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class ListDrawerSettingsAttribute : OdinShimAttribute
    {
        public bool IsReadOnly { get; set; }
        public bool DraggableItems { get; set; }
        public int NumberOfItemsPerPage { get; set; }
        public string CustomAddFunction { get; set; }
    }

    /// <summary>Inspector 提示条。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class InfoBoxAttribute : OdinShimAttribute
    {
        public InfoBoxAttribute(string message) { }
    }

    /// <summary>值变更回调，参数为方法名。</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class OnValueChangedAttribute : OdinShimAttribute
    {
        public OnValueChangedAttribute(string methodName) { }
    }

    /// <summary>ValueDropdown 的单项：显示文本 + 实际值。</summary>
    public struct ValueDropdownItem<T>
    {
        public string Text;
        public T Value;

        public ValueDropdownItem(string text, T value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Text}: {Value}";
        }
    }

    /// <summary>
    /// ValueDropdown 的候选值容器。Odin 中继承自 List&lt;ValueDropdownItem&lt;T&gt;&gt;，
    /// 额外提供 Add(text, value) 重载。这里保持同样的继承关系，业务代码无需改动。
    /// </summary>
    public class ValueDropdownList<T> : List<ValueDropdownItem<T>>
    {
        public void Add(string text, T value)
        {
            Add(new ValueDropdownItem<T>(text, value));
        }
    }
}
#endif
