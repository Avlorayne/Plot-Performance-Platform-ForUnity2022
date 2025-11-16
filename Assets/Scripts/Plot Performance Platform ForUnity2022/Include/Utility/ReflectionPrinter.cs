using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Plot_Performance_Platform_ForUnity2022.Include.Utility
{
    public interface IPrinter
    {
        string PrintString(object obj);
        string GetAccessModifier(FieldInfo field);

        /// <summary>
        /// 获取对象的所有公共可读属性及其值和类型信息，支持递归处理复合数据结构
        /// </summary>
        Dictionary<(string, string), Type> GetPropers(object obj, int maxDepth = 3, int currentDepth = 0);

        /// <summary>
        /// 获取对象的所有公共字段及其值和类型信息，支持递归处理复合数据结构
        /// </summary>
        Dictionary<(string, string), Type> GetFields(object obj, int maxDepth = 3, int currentDepth = 0);

        string GetMethods(object obj);
        string GetEvents(object obj);

        /// <summary>
        /// 将对象转换为字符串表示，支持递归处理复合数据结构
        /// </summary>
        string ObjectToString(object obj, int maxDepth = 3, int currentDepth = 0);
    }

    public class ReflectionPrinter : IPrinter
    {
        private readonly HashSet<object> _visitedObjects = new HashSet<object>();

        public string PrintString(object obj)
        {
            _visitedObjects.Clear();
            return PrintString(obj, 3, 0);
        }

        private string PrintString(object obj, int maxDepth, int currentDepth)
        {
            if (obj == null) return "[null]";

            Type type = obj.GetType();

            // 打印属性和字段
            StringBuilder Propers = new StringBuilder();
            foreach (var proper in GetPropers(obj, maxDepth, currentDepth))
            {
                Propers.Append($"\t{proper.Value?.Name ?? "unknown"} {proper.Key.Item1}: {proper.Key.Item2} \n");
            }

            StringBuilder Fields = new StringBuilder();
            foreach (var field in GetFields(obj, maxDepth, currentDepth))
            {
                Fields.Append($"\t{field.Value?.Name ?? "unknown"} {field.Key.Item1}: {field.Key.Item2}\n");
            }

            return $@"[{type.Name}]
[Property]: 
{Propers}
[Field]:
{Fields}
[Method]:
{GetMethods(obj)}
[Event]:
{GetEvents(obj)}";
        }

        // 获取字段的访问修饰符
        public string GetAccessModifier(FieldInfo field)
        {
            if (field.IsPublic)
                return "public";
            else if (field.IsPrivate)
                return "private";
            else if (field.IsFamily)
                return "protected";
            else if (field.IsAssembly)
                return "internal";
            else if (field.IsFamilyOrAssembly)
                return "protected internal";
            else
                return "private";
        }

        public Dictionary<(string, string), Type> GetPropers(object obj, int maxDepth = 3, int currentDepth = 0)
        {
            return GetPropersInternal(obj, maxDepth, currentDepth);
        }

        public Dictionary<(string, string), Type> GetFields(object obj, int maxDepth = 3, int currentDepth = 0)
        {
            return GetFieldsInternal(obj, maxDepth, currentDepth);
        }

        private Dictionary<(string, string), Type> GetPropersInternal(object obj, int maxDepth, int currentDepth)
        {
            if (obj == null) return new Dictionary<(string, string), Type>();

            Type type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead);

            Dictionary<(string, string), Type> propersDict = new Dictionary<(string, string), Type>();

            foreach (var prop in properties)
            {
                try
                {
                    var value = prop.GetValue(obj);
                    string valueString = ObjectToString(value, maxDepth, currentDepth + 1);
                    propersDict.Add((prop.Name, valueString), prop.PropertyType);
                }
                catch (Exception ex)
                {
                    propersDict.Add((prop.Name, $"<Error: {ex.Message}>"), null);
                }
            }

            return propersDict;
        }

        private Dictionary<(string, string), Type> GetFieldsInternal(object obj, int maxDepth, int currentDepth)
        {
            if (obj == null) return new Dictionary<(string, string), Type>();

            Type type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => !f.IsDefined(typeof(CompilerGeneratedAttribute), false));

            Dictionary<(string, string), Type> fieldsDict = new Dictionary<(string, string), Type>();

            foreach (var field in fields)
            {
                var accessModifier = GetAccessModifier(field);
                if (accessModifier != "public")
                    continue;

                try
                {
                    var value = field.GetValue(obj);
                    string valueString = ObjectToString(value, maxDepth, currentDepth + 1);
                    fieldsDict.Add((field.Name, valueString), field.FieldType);
                }
                catch (Exception ex)
                {
                    fieldsDict.Add((field.Name, $"<Error: {ex.Message}>"), null);
                }
            }

            return fieldsDict;
        }

        /// <summary>
        /// 将对象转换为字符串表示，支持递归处理复合数据结构
        /// </summary>
        public string ObjectToString(object obj, int maxDepth = 3, int currentDepth = 0)
        {
            if (obj == null)
                return "null";

            // 防止循环引用
            if (_visitedObjects.Contains(obj))
                return "<circular reference>";

            // 检查递归深度
            if (currentDepth >= maxDepth)
                return $"<max depth {maxDepth} reached>";

            Type type = obj.GetType();

            // 处理基本类型和字符串
            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                return obj.ToString();

            // 处理Unity对象
            if (obj is UnityEngine.Object unityObj)
            {
                if (unityObj == null)
                    return "null";
                return $"{unityObj.GetType().Name}({unityObj.name})";
            }

            // 处理数组和集合
            if (obj is System.Collections.IEnumerable enumerable && !(obj is string))
            {
                return CollectionToString(enumerable, maxDepth, currentDepth);
            }

            // 处理复杂对象 - 递归探入
            try
            {
                _visitedObjects.Add(obj);
                return ComplexObjectToString(obj, maxDepth, currentDepth);
            }
            finally
            {
                if (currentDepth == 0)
                {
                    _visitedObjects.Clear();
                }
            }
        }

        private string CollectionToString(System.Collections.IEnumerable collection, int maxDepth, int currentDepth)
        {
            var elements = new List<string>();
            int count = 0;
            const int maxElements = 10;

            foreach (var item in collection)
            {
                if (count >= maxElements)
                {
                    elements.Add("...");
                    break;
                }
                elements.Add(ObjectToString(item, maxDepth, currentDepth + 1));
                count++;
            }

            return $"[{string.Join(", ", elements)}] (Count: {count})";
        }

        private string ComplexObjectToString(object obj, int maxDepth, int currentDepth)
        {
            Type type = obj.GetType();

            // 对于简单值类型，直接返回
            if (type.IsValueType && !type.IsPrimitive && type != typeof(decimal))
            {
                // 检查是否是基础的值类型（如DateTime、Guid等）
                if (type.Namespace?.StartsWith("System") == true)
                    return obj.ToString();
            }

            var properties = new List<string>();

            // 获取公共属性
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .Take(5); // 限制属性数量避免输出过长

            foreach (var prop in props)
            {
                try
                {
                    var value = prop.GetValue(obj);
                    string valueStr = ObjectToString(value, maxDepth, currentDepth + 1);
                    properties.Add($"{prop.Name}: {valueStr}");
                }
                catch (Exception ex)
                {
                    properties.Add($"{prop.Name}: <Error: {ex.Message}>");
                }
            }

            // 获取公共字段
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.IsDefined(typeof(CompilerGeneratedAttribute), false))
                .Take(5); // 限制字段数量

            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(obj);
                    string valueStr = ObjectToString(value, maxDepth, currentDepth + 1);
                    properties.Add($"{field.Name}: {valueStr}");
                }
                catch (Exception ex)
                {
                    properties.Add($"{field.Name}: <Error: {ex.Message}>");
                }
            }

            string content = properties.Any() ? string.Join(", ", properties) : "no public members";
            return $"{type.Name}{{{content}}}";
        }

        // 原有的方法和事件获取方法保持不变
        public string GetMethods(object obj)
        {
            Type type = obj.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName);

            StringBuilder MethodBuilder = new StringBuilder();

            foreach (var method in methods.Take(5))
            {
                var parameters = method.GetParameters();
                var paramString = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                MethodBuilder.Append($"\t{method.ReturnType.Name} {method.Name}({paramString})\n");
            }

            if (methods.Count() > 5)
                MethodBuilder.Append($"\t... and {methods.Count() - 5} more methods\n");

            return MethodBuilder.ToString();
        }

        public string GetEvents(object obj)
        {
            Type type = obj.GetType();
            StringBuilder EventBuilder = new StringBuilder();
            var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance);

            if (events.Any())
            {
                foreach (var evt in events.Take(5))
                {
                    EventBuilder.Append($"\t{evt.Name} ({evt.EventHandlerType?.Name})\n");
                }

                if (events.Count() > 5)
                    EventBuilder.Append($"\t... and {events.Count() - 5} more events\n");
            }
            else
            {
                EventBuilder.Append("\tNo events\n");
            }

            return EventBuilder.ToString();
        }
    }
}
