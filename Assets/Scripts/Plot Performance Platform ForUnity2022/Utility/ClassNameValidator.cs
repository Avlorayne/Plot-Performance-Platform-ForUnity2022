using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Plot_Performance_Platform_ForUnity2022.Utility
{
    public static class ClassNameValidator
    {
        // 保留关键字列表
        private static readonly HashSet<string> ReservedKeywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
            "char", "checked", "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum", "event", "explicit",
            "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "goto", "if", "implicit", "in", "int", "interface", "internal",
            "is", "lock", "long", "namespace", "new", "null", "object",
            "operator", "out", "override", "params", "private", "protected",
            "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string", "struct", "switch",
            "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "virtual", "void",
            "volatile", "while"
        };

        // Unity 特殊类型前缀
        private static readonly HashSet<string> UnitySpecialPrefixes = new HashSet<string>
        {
            "MonoBehaviour", "ScriptableObject", "EditorWindow", "Editor",
            "PropertyDrawer", "Attribute", "NetworkBehaviour"
        };

        /// <summary>
        /// 验证类名是否符合 C# 和 Unity 规范
        /// </summary>
        public static ValidationResult ValidateClassName(string className)
        {
            var result = new ValidationResult { IsValid = true, Errors = new List<string>() };

            if (string.IsNullOrWhiteSpace(className))
            {
                result.IsValid = false;
                result.Errors.Add("类名不能为空");
                return result;
            }

            // 1. 检查长度
            if (className.Length > 50)
            {
                result.IsValid = false;
                result.Errors.Add($"类名过长 ({className.Length} 字符)，建议不超过 50 字符");
            }

            // 2. 检查是否以数字开头
            if (char.IsDigit(className[0]))
            {
                result.IsValid = false;
                result.Errors.Add("类名不能以数字开头");
            }

            // 3. 检查是否包含无效字符
            if (!Regex.IsMatch(className, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                result.IsValid = false;
                result.Errors.Add("类名只能包含字母、数字和下划线，且不能以数字开头");
            }

            // 4. 检查是否为保留关键字
            if (ReservedKeywords.Contains(className.ToLower()))
            {
                result.IsValid = false;
                result.Errors.Add($"'{className}' 是 C# 保留关键字");
            }

            // // 5. 检查命名风格（PascalCase）
            // if (!IsPascalCase(className))
            // {
            //     result.IsValid = false;
            //     result.Errors.Add("类名应该使用 PascalCase 命名风格（首字母大写，每个单词首字母大写）");
            // }

            // // 6. 检查是否有意义
            // if (IsMeaninglessName(className))
            // {
            //     result.Warnings.Add("类名应该具有描述性，避免使用无意义的名称");
            // }

            // 7. Unity 特定检查
            CheckUnityConventions(className, result);

            return result;
        }

        // /// <summary>
        // /// 检查是否符合 PascalCase 命名规范
        // /// </summary>
        // private static bool IsPascalCase(string name)
        // {
        //     if (string.IsNullOrEmpty(name)) return false;
        //
        //     // 首字母必须大写
        //     if (!char.IsUpper(name[0])) return false;
        //
        //     // 不能包含连续大写（除了缩写）
        //     for (int i = 1; i < name.Length - 1; i++)
        //     {
        //         if (char.IsUpper(name[i]) && char.IsUpper(name[i + 1]))
        //         {
        //             // 允许常见的缩写如 UI, AI, RPG 等
        //             string possibleAbbreviation = name.Substring(i, 2);
        //             if (!IsCommonAbbreviation(possibleAbbreviation))
        //                 return false;
        //         }
        //     }
        //
        //     return true;
        // }

        // /// <summary>
        // /// 常见缩写检查
        // /// </summary>
        // private static bool IsCommonAbbreviation(string abbreviation)
        // {
        //     string[] commonAbbreviations = { "UI", "UX", "AI", "RPG", "FPS", "API", "SQL", "XML", "JSON", "HTTP" };
        //     return Array.Exists(commonAbbreviations, abbr => abbr == abbreviation);
        // }

        // /// <summary>
        // /// 检查是否为无意义名称
        // /// </summary>
        // private static bool IsMeaninglessName(string name)
        // {
        //     string[] meaninglessNames = { "Test", "Temp", "New", "Old", "Class1", "MyClass", "Example" };
        //     return Array.Exists(meaninglessNames, meaningless =>
        //         name.Equals(meaningless, StringComparison.OrdinalIgnoreCase) ||
        //         name.StartsWith(meaningless, StringComparison.OrdinalIgnoreCase));
        // }

        /// <summary>
        /// Unity 特定规范检查
        /// </summary>
        private static void CheckUnityConventions(string className, ValidationResult result)
        {
            // 检查是否应该包含 Unity 特定后缀
            bool isUnityComponent = className.EndsWith("Behaviour") ||
                                    className.EndsWith("Component") ||
                                    className.EndsWith("Manager") ||
                                    className.EndsWith("System") ||
                                    className.EndsWith("Controller");

            if (isUnityComponent && !className.EndsWith("Behaviour") &&
                !className.EndsWith("Component"))
            {
                result.Suggestions.Add($"Unity 组件建议使用 'Behaviour' 或 'Component' 后缀，例如: {className}Behaviour");
            }

            // 检查编辑器类命名
            if (className.Contains("Editor") && !className.EndsWith("Editor"))
            {
                result.Suggestions.Add($"编辑器类建议以 'Editor' 结尾，例如: {className}Editor");
            }

            // 检查属性绘制器命名
            if (className.Contains("Drawer") && !className.EndsWith("Drawer"))
            {
                result.Suggestions.Add($"属性绘制器建议以 'Drawer' 结尾，例如: {className}Drawer");
            }
        }

        /// <summary>
        /// 生成建议的类名
        /// </summary>
        public static string GenerateSuggestedName(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName))
                return "MyClass";

            // 移除无效字符
            string cleaned = Regex.Replace(inputName, @"[^a-zA-Z0-9_]", "");

            // 确保首字母大写
            if (cleaned.Length > 0)
            {
                cleaned = char.ToUpper(cleaned[0]) + cleaned.Substring(1);
            }
            else
            {
                cleaned = "MyClass";
            }

            // 如果是保留关键字，添加后缀
            if (ReservedKeywords.Contains(cleaned.ToLower()))
            {
                cleaned += "Class";
            }

            return cleaned;
        }
    }

    /// <summary>
    /// 验证结果类
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Suggestions { get; set; } = new List<string>();
    }
}
