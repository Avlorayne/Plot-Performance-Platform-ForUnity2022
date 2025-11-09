using System;
using System.Collections.Generic;
using System.Linq;
using Plot_Performance_Platform_ForUnity2022.Controller;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using UnityEngine.UIElements;

namespace Plot_Performance_Platform_ForUnity2022.EditorPanel
{
public class ParamForView
{
    private static Dictionary<Type, Type> paramToUI =
        new Dictionary<Type, Type>
        {
            { typeof(int), typeof(TextField)}
        };

    public Type UIType {private set; get;}
    public Type paramType {set; get;}
    public string name {set; get;}

    ParamForView(string name, Type paramType)
    {
        this.name = name;
        this.paramType = paramType;
        UIType = paramToUI.TryGetValue(this.paramType, out var uiType) ? uiType : null;
    }
}

public class PlotView: List<string>
{
    private InstrList instrList { get; set; }

    // 类型和参数的注册表
    Dictionary<Type, ParamForView[]> paramForView = new ();

    private Type[] GetInstrTypes()
    {
        Type baseType = typeof(InstrParam);

        var allSubTypes = baseType.Assembly.GetTypes().Where(t =>baseType.IsAssignableFrom(t) && t != baseType && t.IsClass && !t.IsAbstract);
        return allSubTypes.ToArray();
    }

}
}
