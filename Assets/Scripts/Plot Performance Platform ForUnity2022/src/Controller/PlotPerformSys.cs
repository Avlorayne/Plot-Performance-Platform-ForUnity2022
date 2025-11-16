using System.IO;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using Plot_Performance_Platform_ForUnity2022.src.Allocate;
using Plot_Performance_Platform_ForUnity2022.src.DataSequence;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Plot_Performance_Platform_ForUnity2022.src.Controller
{
    public class PlotPerformSys : MonoBehaviour
    {
        public FrameList frameList = new FrameList();
        public FrameExecuteList FrameExecuteList;
        public TextAsset plotJson;
        public Button saveButton;

        void Start()
        {
            // 使用新的 Frame 封装
            frameList.Add(new InstrParam[]{new TypeDialogueParam(new Dialogue("name","sentence"))});
            frameList.Print();
            // Serialize();
            FrameExecuteList = new FrameExecuteList(frameList);
            FrameExecuteList.Print();
        }

        void Update()
        {
        }

        void DeSerialize()
        {
            string json = plotJson.text.Trim('\uFEFF');
            frameList.Deserialize(json);
            frameList.Print();
        }

        void Serialize()
        {
            string json = frameList.Serialize();
            string path = AssetDatabase.GetAssetPath(plotJson);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        public void ButtonSave()
        {
            Serialize();
            DeSerialize();
        }
    }
}
