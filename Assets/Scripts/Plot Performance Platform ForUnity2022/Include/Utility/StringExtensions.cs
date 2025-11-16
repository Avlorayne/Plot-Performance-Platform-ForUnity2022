using System;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Utility
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 将字符串截断到指定长度，如果超长则添加省略号
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="maxLength">最大长度（包含省略号）</param>
        /// <param name="ellipsis">省略号字符串，默认为"..."</param>
        /// <returns>截断后的字符串</returns>
        public static string Truncate(this string str, int maxLength, string ellipsis = "...")
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (maxLength <= 0)
                return string.Empty;

            // 如果省略号长度已经超过最大长度，直接返回省略号
            if (ellipsis.Length >= maxLength)
                return ellipsis.Length > maxLength ? ellipsis.Substring(0, maxLength) : ellipsis;

            // 如果字符串本身不超过最大长度，直接返回
            if (str.Length <= maxLength)
                return str;

            // 计算截取长度（最大长度减去省略号长度）
            int truncateLength = maxLength - ellipsis.Length;
            
            // 确保截取长度不小于1
            if (truncateLength <= 0)
                return ellipsis;

            return str.Substring(0, truncateLength) + ellipsis;
        }

        /// <summary>
        /// 安全地截取字符串，避免索引越界
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="length">截取长度</param>
        /// <returns>截取后的字符串</returns>
        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (startIndex < 0) startIndex = 0;
            if (startIndex >= str.Length) return string.Empty;

            int actualLength = Math.Min(length, str.Length - startIndex);
            return str.Substring(startIndex, actualLength);
        }

        /// <summary>
        /// 获取字符串的预览（前N个字符）
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="previewLength">预览长度</param>
        /// <param name="ellipsis">省略号</param>
        /// <returns>预览字符串</returns>
        public static string Preview(this string str, int previewLength, string ellipsis = "...")
        {
            return str.Truncate(previewLength, ellipsis);
        }
    }
}
