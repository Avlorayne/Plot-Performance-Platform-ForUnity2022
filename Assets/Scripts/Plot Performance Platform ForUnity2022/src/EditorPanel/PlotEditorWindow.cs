using Plot_Performance_Platform_ForUnity2022.Construct;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plot_Performance_Platform_ForUnity2022.src.EditorPanel
{
public class PlotEditorWindow: EditorWindow
{
    private ScriptTemplateGenerator STG = new ScriptTemplateGenerator();

    [MenuItem("Window/PlotEditor")]
    public static void ShowExample()
    {
        PlotEditorWindow wnd = GetWindow<PlotEditorWindow>();
        wnd.titleContent = new GUIContent("PlotEditor");
    }

    public void CreateGUI()
    {
        // 每个编辑器窗口包含一个根 VisualElement 对象
        VisualElement root = rootVisualElement;

        // VisualElement 对象可按树状层次包含其他元素
        Label messege = new Label("");
        root.Add(messege);

        // 输入创建指令脚本名----------------------------------------
        Label SN = new Label("Instruction Name:");
        root.Add(SN);

        TextField scriptName = new TextField();
        root.Add(scriptName);

        // 创建按钮------------------------------------------------
        Button createButton = new Button();
        createButton.name = "CreateButton";
        createButton.text = "Create";
        createButton.clicked += () =>
        {
            string currentScriptName = scriptName.text.Trim(); // 先保存名称
            if (currentScriptName != "")
            {
                bool result = STG.CreateScript(currentScriptName);
                if (result)
                {
                    scriptName.value = "";
                    // 【关键修改】使用保存的名称，而不是清空后的 scriptName.text
                    messege.text = $"Managed to Create {currentScriptName}.cs !\n";
                }
                else
                {
                    messege.text = $"Failed to Create {currentScriptName}.cs !\nPlease Check If the Folder Exists or the Script Name is Valid.\n";
                }
            }
        };
        root.Add(createButton);


        // ----------------------------------------------
    }


}
}
