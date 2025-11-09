using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plot_Performance_Platform_ForUnity2022.Utility;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Instruction
{
public class InstrParam
{
    private static ISerializer _serializer = new ReflectionSerializer();
    private static IPrinter _printer = new ReflectionPrinter();

    public virtual string Name { get; set; } = "SerializeJson.InstrParam";
    // {
    //     get => throw new  NotImplementedException("Name must be implemented!\n指令名必须被重写");
    //     set => throw new  NotImplementedException("Name must be implemented!\n指令名必须被重写");
    // }
    public virtual string Description { get; set; } = "Basic Description";
    // {
    //     get => throw new  NotImplementedException("Description must be implemented!\n指令描述必须被重写");
    //     set => throw new  NotImplementedException("Description must be implemented!\n指令描述必须被重写");
    // }
    // 共存数量
    public virtual int CoexistingQuantity { get; set; } = 1;
    [JsonExtensionData]
    protected Dictionary<string, JsonElement>? ExtensionData { get; set; } = new();

    #region Print
    public static void Print(InstrParam instrParam)
    {
        _printer.Print(instrParam);
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
        Debug.Log($"Start to Convert: {instrParam.Name}");
        string jsonString = JsonSerializer.Serialize(instrParam);
        string typeName = instrParam.Name;

        Debug.Log(typeName);

        // 确保类型名称包含命名空间
        if (!typeName.Contains("Plot_Performance_Platform_ForUnity2022.Instruction."))
        {
            typeName = "Plot_Performance_Platform_ForUnity2022.Instruction." + typeName;
            Debug.Log(typeName);
        }

        // 从当前程序集查找类型
        Type type = Type.GetType(typeName) ?? Assembly.GetExecutingAssembly().GetType(typeName);

        Debug.Log($"Find Type :{type.FullName}");

        if (type == null)
        {
            throw new InvalidOperationException($"Type '{typeName}' not found.");
        }

        return JsonSerializer.Deserialize(jsonString, type) as InstrParam;
    }
    #endregion
}
}
