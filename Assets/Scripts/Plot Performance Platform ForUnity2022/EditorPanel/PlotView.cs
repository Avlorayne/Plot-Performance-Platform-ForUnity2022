using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plot_Performance_Platform_ForUnity2022.Controller;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using Plot_Performance_Platform_ForUnity2022.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plot_Performance_Platform_ForUnity2022.EditorPanel
{
public class ParamForView
{
    // 参数类型与对应输入UI
    private static Dictionary<Type, Type> paramToUI =
        new ()
        {
            { typeof(int), typeof(TextField)}
        };

    public Type UIType {private set; get;}
    private Type paramType {set; get;}
    public string name {set; get;}

    public ParamForView(string name, Type paramType)
    {
        this.name = name;
        this.paramType = paramType;
        UIType = paramToUI.GetValueOrDefault(this.paramType);
    }
}

public class PlotView
{
    private IPrinter _printer = new ReflectionPrinter();
    private FrameList instrList { get; set; }

    // 类型和参数的注册表
    Dictionary<Type, ParamForView[]> paramDict = new ();

    private Type[] GetInstrTypes(Type baseType)
    {
        var allSubTypes = baseType.Assembly.GetTypes().Where(t =>baseType.IsAssignableFrom(t) && t != baseType && t.IsClass && !t.IsAbstract);
        return allSubTypes.ToArray();
    }

    private ParamForView[] GetParamForView(Type type)
    {
        List<ParamForView> paramForView = new List<ParamForView>();
        Dictionary<(string,string),Type> properties = _printer.GetPropers(type);
        Dictionary<(string,string),Type> fields = _printer.GetFields(type);

        foreach (var property in properties)
        {
            paramForView.Add(new ParamForView(property.Key.Item1, property.Value));
        }

        foreach (var field in fields)
        {
            paramForView.Add(new ParamForView(field.Key.Item1, field.Value));
        }

        return paramForView.ToArray();
    }

    private void ParamDictConstruct()
    {
        Type[] types = GetInstrTypes(typeof(InstrParam));
        foreach (var type in types)
        {
            paramDict.Add(type, GetParamForView(type));
        }

        StringBuilder paramDictString = new StringBuilder();
        foreach (var paramView in paramDict)
        {
            paramDictString.Append($"\n{paramView.Key}: {paramView.Value}\n");
        }
        Debug.Log(@$"Param Dictionary:
{paramDictString}");
    }

    PlotView()
    {
        ParamDictConstruct();
    }


}
}
