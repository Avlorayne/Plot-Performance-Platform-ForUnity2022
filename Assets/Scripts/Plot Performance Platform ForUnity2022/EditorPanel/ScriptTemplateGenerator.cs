using System;
using System.IO;
using Plot_Performance_Platform_ForUnity2022.Utility;
using UnityEditor;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.EditorPanel
{
public class ScriptTemplateGenerator
{
    private static string templateFile = "Assets/Scripts/Plot Performance Platform ForUnity2022/Instruction/InstructionTemplate.cs.txt";
    private static string folderPath = "Assets/Scripts/Plot Performance Platform ForUnity2022/Instruction/";

    private string template;
    private string scriptName;
    private void ReadTemplate()
    {
        template = File.ReadAllText(templateFile);

        template = template.Replace("#InstrParamTemplate#", $"{scriptName}Param");
        template = template.Replace("#InstrExecuteTemplate#", $"{scriptName}Execute");
    }

    public bool CreateScript(string scriptName)
    {
        // 验证类名是否合法
        ValidationResult result = ClassNameValidator.ValidateClassName(scriptName);
        if (!result.IsValid)
        {
            foreach (string error in result.Errors)
                Debug.Log($"  - {error}");
            return false;
        }

        this.scriptName = scriptName;
        ReadTemplate();
        string scriptPath = $"{folderPath}{this.scriptName}.cs";

        try
        {
            // 检查文件是否已存在
            if (File.Exists(scriptPath))
            {
                Debug.LogWarning($"文件 {this.scriptName}.cs 已存在！");
                return false;
            }

            // 创建文件
            File.WriteAllText(scriptPath, template);

            // 【关键修改】先刷新资产数据库
            AssetDatabase.Refresh();

            // 然后检查文件是否存在
            if (File.Exists(scriptPath))
            {
                Debug.Log($"{this.scriptName}.cs 文件创建成功！");
                return true;
            }
            else
            {
                Debug.Log($"文件创建后检查失败");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"创建文件失败: {ex.Message}");
            return false;
        }
    }

}
}
