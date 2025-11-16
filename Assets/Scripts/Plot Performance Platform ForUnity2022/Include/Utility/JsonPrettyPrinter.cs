using System.Text;
using System.Text.Json;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Include.Utility
{
    /// <summary>
    /// JSON字符串优质打印工具类
    /// 提供格式化和美化JSON字符串的功能
    /// </summary>
    public static class JsonPrettyPrinter
    {
        private const string INDENT_STRING = "    "; // 4个空格作为缩进
        private const string COLOR_OBJECT = "#FF6B6B";    // 对象 - 红色
        private const string COLOR_ARRAY = "#4ECDC4";     // 数组 - 青色
        private const string COLOR_STRING = "#45B7D1";    // 字符串 - 蓝色
        private const string COLOR_NUMBER = "#96CEB4";    // 数字 - 绿色
        private const string COLOR_BOOLEAN = "#FFEAA7";   // 布尔值 - 黄色
        private const string COLOR_NULL = "#DDA0DD";      // null - 紫色
        private const string COLOR_KEY = "#FDCB6E";       // 键 - 橙色

        /// <summary>
        /// 格式化JSON字符串（无颜色）
        /// </summary>
        /// <param name="json">原始JSON字符串</param>
        /// <returns>格式化后的JSON字符串</returns>
        public static string Format(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            try
            {
                using var document = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                return JsonSerializer.Serialize(document.RootElement, options);
            }
            catch (JsonException)
            {
                // 如果标准方法失败，使用自定义格式化
                return FormatManually(json);
            }
        }

        /// <summary>
        /// 格式化并添加颜色高亮（适用于Unity控制台）
        /// </summary>
        /// <param name="json">原始JSON字符串</param>
        /// <returns>带颜色标签的格式化JSON字符串</returns>
        public static string FormatWithColor(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            var formatted = Format(json);
            return AddColorHighlight(formatted);
        }

        /// <summary>
        /// 打印到Unity控制台（带颜色）
        /// </summary>
        /// <param name="json">原始JSON字符串</param>
        /// <param name="title">可选的标题</param>
        public static void PrintToConsole(string json, string title = null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Debug.Log($"<b><color=white>=== {title} ===</color></b>");
            }

            var coloredJson = FormatWithColor(json);
            Debug.Log(coloredJson);
        }

        /// <summary>
        /// 验证并格式化JSON
        /// </summary>
        /// <param name="json">原始JSON字符串</param>
        /// <returns>格式化结果</returns>
        public static (bool isValid, string formatted, string error) ValidateAndFormat(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return (false, json, "JSON string is null or empty");

            try
            {
                // 验证JSON有效性
                using var document = JsonDocument.Parse(json);
                var formatted = Format(json);
                return (true, formatted, null);
            }
            catch (JsonException ex)
            {
                return (false, json, $"Invalid JSON: {ex.Message}");
            }
        }

        /// <summary>
        /// 手动格式化JSON（备用方法）
        /// </summary>
        private static string FormatManually(string json)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();

            for (var i = 0; i < json.Length; i++)
            {
                var ch = json[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            indent++;
                            sb.Append(INDENT_STRING.Repeat(indent));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            indent--;
                            sb.Append(INDENT_STRING.Repeat(indent));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        var escaped = false;
                        var index = i;
                        while (index > 0 && json[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            sb.Append(INDENT_STRING.Repeat(indent));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(' ');
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 添加颜色高亮
        /// </summary>
        private static string AddColorHighlight(string json)
        {
            var result = new StringBuilder();
            var inString = false;
            var escaped = false;
            var inKey = false;

            for (int i = 0; i < json.Length; i++)
            {
                var ch = json[i];

                // 处理转义字符
                if (ch == '\\' && inString)
                {
                    escaped = !escaped;
                    result.Append(ch);
                    continue;
                }

                if (ch == '"' && !escaped)
                {
                    if (inString)
                    {
                        // 字符串结束
                        if (inKey)
                        {
                            result.Append($"</color>");
                            inKey = false;
                        }
                        else
                        {
                            result.Append($"</color>");
                        }
                        inString = false;
                    }
                    else
                    {
                        // 字符串开始
                        inString = true;
                        // 检查是否是键（后面跟着冒号）
                        var isKey = i + 1 < json.Length && json[i + 1] == ':';
                        if (isKey)
                        {
                            result.Append($"<color={COLOR_KEY}>\"");
                            inKey = true;
                        }
                        else
                        {
                            result.Append($"<color={COLOR_STRING}>\"");
                        }
                    }
                    result.Append(ch);
                    continue;
                }

                if (escaped)
                {
                    escaped = false;
                }

                if (!inString)
                {
                    // 处理非字符串内容
                    switch (ch)
                    {
                        case '{':
                        case '}':
                            result.Append($"<color={COLOR_OBJECT}>{ch}</color>");
                            break;
                        case '[':
                        case ']':
                            result.Append($"<color={COLOR_ARRAY}>{ch}</color>");
                            break;
                        case 't': // true
                        case 'f': // false
                            if (IsBoolean(json, i))
                            {
                                var boolean = json.Substring(i, ch == 't' ? 4 : 5);
                                result.Append($"<color={COLOR_BOOLEAN}>{boolean}</color>");
                                i += ch == 't' ? 3 : 4;
                            }
                            else
                            {
                                result.Append(ch);
                            }
                            break;
                        case 'n': // null
                            if (IsNull(json, i))
                            {
                                result.Append($"<color={COLOR_NULL}>null</color>");
                                i += 3;
                            }
                            else
                            {
                                result.Append(ch);
                            }
                            break;
                        default:
                            if (char.IsDigit(ch) || ch == '-')
                            {
                                var number = ExtractNumber(json, i);
                                result.Append($"<color={COLOR_NUMBER}>{number}</color>");
                                i += number.Length - 1;
                            }
                            else
                            {
                                result.Append(ch);
                            }
                            break;
                    }
                }
                else
                {
                    result.Append(ch);
                }
            }

            return result.ToString();
        }

        private static bool IsBoolean(string json, int index)
        {
            if (json[index] == 't' && index + 3 < json.Length)
                return json.Substring(index, 4) == "true";
            if (json[index] == 'f' && index + 4 < json.Length)
                return json.Substring(index, 5) == "false";
            return false;
        }

        private static bool IsNull(string json, int index)
        {
            return index + 3 < json.Length && json.Substring(index, 4) == "null";
        }

        private static string ExtractNumber(string json, int index)
        {
            var end = index;
            while (end < json.Length && (char.IsDigit(json[end]) || json[end] == '.' || json[end] == 'e' || json[end] == 'E' || json[end] == '+' || json[end] == '-'))
            {
                end++;
            }
            return json.Substring(index, end - index);
        }
    }

    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static partial class StringExtensions
    {
        public static string Repeat(this string str, int count)
        {
            if (count <= 0) return string.Empty;
            if (count == 1) return str;

            var sb = new StringBuilder(str.Length * count);
            for (int i = 0; i < count; i++)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }
    }
}
