using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Plot_Performance_Platform_ForUnity2022.Include.Utility;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Include.Construct
{
[Serializable]
public class InstrParam
{
    private static ISerializer _serializer = new ReflectionSerializer();
    private static IPrinter _printer = new ReflectionPrinter();
    public static string Namespace = "Plot_Performance_Platform_ForUnity2022.Instruction.";

    #region Const
    [JsonInclude] public virtual string Name { get; protected set; } = nameof(InstrParam);
    [JsonInclude] protected virtual string _ExecutorType { get; set; } = nameof(InstrExecute);

    [JsonIgnore]
    public Type ExecutorType
    {
        get
        {
            string typeName = _ExecutorType;
            // 确保类型名称包含命名空间
            if (!typeName.Contains(Namespace))
            {
                typeName = Namespace + typeName;
            }

            // 从当前程序集查找类型
            Type type = Type.GetType(typeName) ?? Assembly.GetExecutingAssembly().GetType(typeName);

            if (type == null)
            {
                Debug.Log($"[InstrParam.ExecutorType]Can't find executor type {_ExecutorType}");
            }
            Debug.Log($"[InstrParam.ExecutorType]Find  executor type {type.Name}");
            return type;
        }
        protected set{ }
    }

    #endregion

    [JsonInclude] public virtual string Description { get; protected set; } = "Basic Description";
    [JsonInclude] public virtual bool IsCanCoexist { get; set; } = false;
    [JsonInclude] public virtual bool IsRelese { get; set; } = false;

    [JsonInclude] public virtual bool IsCanBeSkipped { get; set; } = false;

    [JsonInclude] [JsonExtensionData] [CanBeNull] protected Dictionary<string, JsonElement> ExtensionData { get; set; } = new();


    #region Print

    public static string PrintString(InstrParam instrParam)
    {
        return _printer.PrintString(instrParam);
    }

    #endregion

    // 序列化部分 ---------------
    #region JsonSerialize

    public static string Serialize(InstrParam instrParam)
    {
        return _serializer.Serialize(instrParam);
    }

    public static T Deserialize<T>(string jsonString) where T : InstrParam
    {
        T instrParam = JsonSerializer.Deserialize<T>(jsonString);
        return instrParam;
    }

    public static InstrParam Convert(InstrParam instrParam)
    {
        Debug.Log($"[InstrParam.Convert]Json String\n{_printer.PrintString(instrParam)}");

        string jsonString = JsonSerializer.Serialize(instrParam);
        string typeName = instrParam.Name;

        // 确保类型名称包含命名空间
        if (!typeName.Contains(Namespace))
        {
            typeName = Namespace + typeName;
        }

        // 从当前程序集查找类型
        Type type = Type.GetType(typeName) ?? Assembly.GetExecutingAssembly().GetType(typeName);

        if (type == null)
        {
            throw new InvalidOperationException($"Type '{typeName}' not found.");
        }

        Debug.Log($"[InstrParam.Convert]Find Type :{type.Name}");

        return JsonSerializer.Deserialize(jsonString,
            type,
            new JsonSerializerOptions
            {
                IncludeFields = true
            }
            )
            as InstrParam;
    }

    #endregion

    #region CheckValid

    // private bool CheckValid()
    // {
    //
    // }

    #endregion
}
}
