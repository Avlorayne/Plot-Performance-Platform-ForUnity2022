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
    void Print(object obj);
    string GetAccessModifier(FieldInfo field);
    Dictionary<(string, string), Type> GetPropers(object obj);
    Dictionary<(string, string), Type> GetFields(object obj);
    string GetMethods(object obj);
    string GetEvents(object obj);
}

public class ReflectionPrinter : IPrinter
{
    public  void Print(object obj)
    {
        Type type = obj.GetType();
        Debug.Log($"{type.Name}------------------------------------------------------");
        // 打印属性和字段
        StringBuilder Propers = new StringBuilder();
        foreach (var proper in GetPropers(obj))
        {
            Propers.Append($"\t{proper.Value.Name} {proper.Key.Item1}: {proper.Key.Item2} \n");
        }
        Debug.Log(@$"[{type.Name} Property]:
{Propers}");

        StringBuilder Fields =  new StringBuilder();
        foreach (var field in GetFields(obj))
        {
            Fields.Append($"\t{field.Value.Name} {field.Key.Item1}: {field.Key.Item2}\n");
        }
        Debug.Log(@$"[{type.Name} Field]:
{Fields}");

        // 打印方法和事件（可选扩展）
        Debug.Log(@$"[{type.Name} Method]:
{GetMethods(obj)}");

        Debug.Log(@$"[{type.Name} Event]:
{GetEvents(obj)}");
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
