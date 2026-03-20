using System;

namespace Knot.Include.Construct
{
    /// <summary>
    /// 标记字段或属性需要在编辑器中显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DisplayInEditorAttribute : Attribute
    {
        /// <summary>
        /// 显示名称（如果为空则使用字段/属性名）
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 显示顺序（小的在前）
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// 工具提示文本
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// 是否在折叠面板中显示（对于复杂类型）
        /// </summary>
        public bool ShowAsFoldout { get; set; } = true;

        /// <summary>
        /// 是否隐藏该字段（用于临时调试或条件显示）
        /// </summary>
        public bool Hidden { get; set; } = false;

        /// <summary>
        /// 最小限制值（用于数值类型）
        /// </summary>
        public float MinValue { get; set; } = float.MinValue;

        /// <summary>
        /// 最大限制值（用于数值类型）
        /// </summary>
        public float MaxValue { get; set; } = float.MaxValue;

        public DisplayInEditorAttribute() { }

        public DisplayInEditorAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public DisplayInEditorAttribute(string displayName, int order)
        {
            DisplayName = displayName;
            Order = order;
        }

        public DisplayInEditorAttribute(string displayName, int order, string tooltip)
        {
            DisplayName = displayName;
            Order = order;
            Tooltip = tooltip;
        }
    }
}
