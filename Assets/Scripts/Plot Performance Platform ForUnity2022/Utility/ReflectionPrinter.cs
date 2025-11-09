using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Utility
{
public interface IPrinter
{
    string PrintString(object obj);
    string GetAccessModifier(FieldInfo field);
    /// <summary>
    /// 获取对象的所有公共可读属性及其值和类型信息
    /// </summary>
    /// <param name="obj">要反射的目标对象</param>
    /// <returns>
    /// 返回一个字典，其中：
    /// - 键：(属性名称, 属性值的字符串表示)
    /// - 值：属性的类型
    /// 如果获取属性值时发生异常，值类型将为null，并在值字符串中包含错误信息
    /// </returns>
    /// <remarks>
    /// 此方法通过反射获取对象的所有公共实例属性，包括：
    /// - 属性名称
    /// - 属性值的字符串表示（调用ToString()）
    /// - 属性的实际类型
    /// 对于无法读取或访问的属性值，会在字典中记录错误信息
    /// </remarks>
    /// <example>
    /// 示例用法：
    /// <code>
    /// var person = new Person { Name = "John", Age = 25 };
    /// var properties = GetPropers(person);
    /// foreach (var prop in properties)
    /// {
    ///     Console.WriteLine($"Name: {prop.Key.Item1}, Value: {prop.Key.Item2}, Type: {prop.Value}");
    /// }
    /// </code>
    /// </example>
    Dictionary<(string, string), Type> GetPropers(object obj);

    /// <summary>
    /// 获取对象的所有公共字段及其值和类型信息
    /// </summary>
    /// <param name="obj">要反射的目标对象</param>
    /// <returns>
    /// 返回一个字典，其中：
    /// - 键：(字段名称, 字段值的字符串表示)
    /// - 值：字段的类型
    /// 如果获取字段值时发生异常，值类型将为null，并在值字符串中包含错误信息
    /// </returns>
    /// <remarks>
    /// 此方法通过反射获取对象的所有实例字段，但仅返回公共字段，包括：
    /// - 字段名称
    /// - 字段值的字符串表示（调用ToString()）
    /// - 字段的实际类型
    /// 方法会排除编译器生成的字段（如自动属性对应的字段）和非公共字段
    /// 对于无法访问的字段值，会在字典中记录错误信息
    /// </remarks>
    /// <example>
    /// 示例用法：
    /// <code>
    /// var myClass = new MyClass();
    /// var fields = GetFields(myClass);
    /// foreach (var field in fields)
    /// {
    ///     Console.WriteLine($"Name: {field.Key.Item1}, Value: {field.Key.Item2}, Type: {field.Value}");
    /// }
    /// </code>
    /// </example>
    Dictionary<(string, string), Type> GetFields(object obj);
    string GetMethods(object obj);
    string GetEvents(object obj);
}

public class ReflectionPrinter : IPrinter
{
    public string PrintString(object obj)
    {
        Type type = obj.GetType();

        // 打印属性和字段
        StringBuilder Propers = new StringBuilder();
        foreach (var proper in GetPropers(obj))
        {
            Propers.Append($"\t{proper.Value.Name} {proper.Key.Item1}: {proper.Key.Item2} \n");
        }


        StringBuilder Fields =  new StringBuilder();
        foreach (var field in GetFields(obj))
        {
            Fields.Append($"\t{field.Value.Name} {field.Key.Item1}: {field.Key.Item2}\n");
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

    public Dictionary<(string, string), Type> GetPropers(object obj)
    {
        Type type = obj.GetType();
        // 序列化属性
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        Dictionary<(string, string), Type> propersDict = new ();


        foreach (var prop in properties)
        {
            try
            {
                var value = prop.GetValue(obj);
                propersDict.Add((prop.Name, value.ToString()), prop.PropertyType);
            }
            catch (Exception ex)
            {
                propersDict.Add((prop.Name, $"<Error: {ex.Message}>"), null);
            }
        }

        return propersDict;
    }

    public Dictionary<(string, string), Type> GetFields(object obj)
    {
        Type type = obj.GetType();
        // 序列化字段
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => !f.IsDefined(typeof(CompilerGeneratedAttribute), false)); // 排除编译器生成的字段

        Dictionary<(string, string), Type> fieldsDict = new ();

        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            var accessModifier = GetAccessModifier(field);
            if(accessModifier != "public")
                continue;
            try
            {
                fieldsDict.Add((field.Name, value.ToString()), field.FieldType);
            }
            catch (Exception ex)
            {
                fieldsDict.Add((field.Name, $"<Error: {ex.Message}>"), null);
            }
        }

        return fieldsDict;
    }

    // 扩展：打印方法和事件
    public string GetMethods(object obj)
    {
        Type type = obj.GetType();
        // 获取方法
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName); // 排除属性getter/setter等方法

        StringBuilder MethodBuilder = new StringBuilder();

        foreach (var method in methods.Take(5)) // 限制数量避免输出过多
        {
            var parameters = method.GetParameters();
            var paramString = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            MethodBuilder.Append($"\t{method.ReturnType.Name} {method.Name}({paramString})");
        }
        if (methods.Count() > 5)
            MethodBuilder.Append($"\t... and {methods.Count() - 5} more methods");

        return MethodBuilder.ToString();
    }

    public string GetEvents(object obj)
    {
        Type type = obj.GetType();

        StringBuilder EventBuilder = new StringBuilder();
        // 获取事件
        var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance);

        if (events.Any())
        {
            foreach (var evt in events)
            {
                EventBuilder.Append($"\t{evt.Name} ({evt.EventHandlerType?.Name})");
            }
        }
        return EventBuilder.ToString();
    }
}
}
