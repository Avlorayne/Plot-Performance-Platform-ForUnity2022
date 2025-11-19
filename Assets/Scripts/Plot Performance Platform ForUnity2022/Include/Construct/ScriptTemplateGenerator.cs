using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plot_Performance_Platform_ForUnity2022.Include.Utility;
using UnityEditor;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Include.Construct
{
public class ScriptTemplateGenerator
{
    private string fileStorePath =
        "Assets/Scripts/Plot Performance Platform ForUnity2022/Include/Construct/ScriptTemplateGenerator.json";
    [JsonInclude] private string templateFile;
    [JsonInclude] private string folderPath;

    private string template;
    private string scriptName;
    private void ReadTemplate()
    {
        GetPath();

        template = File.ReadAllText(templateFile);

        template = template.Replace("_InstrParamTemplate_", $"{scriptName}Param");
        template = template.Replace("_InstrExecuteTemplate_", $"{scriptName}");
    }

    private void GetPath()
    {

        string fileStoreJson = File.ReadAllText(fileStorePath);
        var scriptTemplateGenerator = JsonSerializer.Deserialize<ScriptTemplateGenerator>(fileStoreJson);

        this.templateFile = scriptTemplateGenerator.templateFile;
        this.folderPath = scriptTemplateGenerator.folderPath;
    }


    public bool CreateScript(string scriptName)
    {
        // 验证类名是否合法
        ValidationResult result = ClassNameValidator.ValidateClassName(scriptName);
        if (!result.IsValid)
        {
            foreach (string error in result.Errors)
                Debug.Log($"[ScriptTemplateGenerator]  - {error}");
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
                Debug.LogWarning($"[ScriptTemplateGenerator]文件 {this.scriptName}.cs 已存在！");
                return false;
            }

            // 创建文件
            File.WriteAllText(scriptPath, template);

            // 【关键修改】先刷新资产数据库
            AssetDatabase.Refresh();

            // 然后检查文件是否存在
            if (File.Exists(scriptPath))
            {
                Debug.Log($"[ScriptTemplateGenerator]{this.scriptName}.cs 文件创建成功！");
                return true;
            }
            else
            {
                Debug.Log($"[ScriptTemplateGenerator]文件创建后检查失败");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"[ScriptTemplateGenerator]创建文件失败: {ex.Message}");
            return false;
        }
    }

}
}
