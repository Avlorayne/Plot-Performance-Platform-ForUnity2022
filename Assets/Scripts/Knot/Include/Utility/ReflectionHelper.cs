using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Knot.Include.Construct;

namespace Knot.Include.Utility
{

    public static class ReflectionHelper
    {
        /// <summary>
        /// 获取需要显示在编辑器中的字段（包含DisplayInEditor特性且不隐藏的）
        /// </summary>
        public static List<FieldInfo> GetDisplayableFields(Type type, bool includeInherited = true)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (!includeInherited)
            {
                flags |= BindingFlags.DeclaredOnly;
            }

            var allFields = type.GetFields(flags);
            var displayableFields = new List<FieldInfo>();

            foreach (var field in allFields)
            {
                var displayAttr = field.GetCustomAttribute<DisplayInEditorAttribute>();
                if (displayAttr != null && displayAttr.Hidden)
                    continue;

                // 如果有DisplayInEditor特性，或者有JsonInclude特性且没有JsonIgnore
                if (displayAttr != null ||
                    (field.GetCustomAttribute<JsonIncludeAttribute>() != null &&
                     field.GetCustomAttribute<JsonIgnoreAttribute>() == null))
                {
                    displayableFields.Add(field);
                }
            }

            return displayableFields.OrderBy(f =>
            {
                var attr = f.GetCustomAttribute<DisplayInEditorAttribute>();
                return attr?.Order ?? 0;
            }).ToList();
        }

        /// <summary>
        /// 获取需要显示在编辑器中的属性（包含DisplayInEditor特性且不隐藏的）
        /// </summary>
        public static List<PropertyInfo> GetDisplayableProperties(Type type, bool includeInherited = true)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (!includeInherited)
            {
                flags |= BindingFlags.DeclaredOnly;
            }

            var allProperties = type.GetProperties(flags);
            var displayableProperties = new List<PropertyInfo>();

            foreach (var property in allProperties)
            {
                var displayAttr = property.GetCustomAttribute<DisplayInEditorAttribute>();
                if (displayAttr != null && displayAttr.Hidden)
                    continue;

                // 过滤条件：可写、非索引器、没有JsonIgnore
                if (property.CanWrite &&
                    property.GetIndexParameters().Length == 0 &&
                    property.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                {
                    // 如果有DisplayInEditor特性，或者有JsonInclude特性
                    if (displayAttr != null ||
                        property.GetCustomAttribute<JsonIncludeAttribute>() != null)
                    {
                        displayableProperties.Add(property);
                    }
                }
            }

            return displayableProperties.OrderBy(p =>
            {
                var attr = p.GetCustomAttribute<DisplayInEditorAttribute>();
                return attr?.Order ?? 0;
            }).ToList();
        }

        /// <summary>
        /// 获取成员的显示名称
        /// </summary>
        public static string GetMemberDisplayName(MemberInfo member)
        {
            var displayAttr = member.GetCustomAttribute<DisplayInEditorAttribute>();
            if (!string.IsNullOrEmpty(displayAttr?.DisplayName))
            {
                return displayAttr.DisplayName;
            }

            // 美化字段名：将 _camelCase 或 camelCase 转换为 "Camel Case"
            string name = member.Name;
            if (name.StartsWith("_") && name.Length > 1)
            {
                name = name.Substring(1);
            }

            return System.Text.RegularExpressions.Regex.Replace(
                name,
                "([a-z])([A-Z])",
                "$1 $2"
            );
        }

        /// <summary>
        /// 获取成员的提示文本
        /// </summary>
        public static string GetMemberTooltip(MemberInfo member)
        {
            var displayAttr = member.GetCustomAttribute<DisplayInEditorAttribute>();
            return displayAttr?.Tooltip ?? string.Empty;
        }

        /// <summary>
        /// 获取成员的数值范围限制
        /// </summary>
        public static (float min, float max) GetMemberValueRange(MemberInfo member)
        {
            var displayAttr = member.GetCustomAttribute<DisplayInEditorAttribute>();
            if (displayAttr != null)
            {
                return (displayAttr.MinValue, displayAttr.MaxValue);
            }

            return (float.MinValue, float.MaxValue);
        }

        /// <summary>
        /// 检查成员是否应该显示为折叠面板
        /// </summary>
        public static bool ShouldShowAsFoldout(MemberInfo member)
        {
            var displayAttr = member.GetCustomAttribute<DisplayInEditorAttribute>();
            return displayAttr?.ShowAsFoldout ?? true;
        }
    }
}
