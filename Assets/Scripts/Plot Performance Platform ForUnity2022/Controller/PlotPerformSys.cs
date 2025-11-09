using System.IO;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Plot_Performance_Platform_ForUnity2022.Controller
{
    public class PlotPerformSys : MonoBehaviour
    {
        public InstrList instrList =  new InstrList();
        public TextAsset plotJson;

        public Button saveButton;

        // Start is called before the first frame update
        void Start()
        {
            DeSerialize();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void DeSerialize()
        {
            string json = plotJson.text.Trim('\uFEFF');
            instrList.Deserialize(json);
            instrList.Print();
        }

        void Serialize()
        {
            CPTParam cpt = new  CPTParam();
            instrList.Add(cpt);
            CPT112Param cpt2 = new  CPT112Param();
            instrList.Add(cpt2);

            instrList.Print();
            string json = instrList.Serialize();

            string path = AssetDatabase.GetAssetPath(plotJson);
            File.WriteAllText(path, json);

            Debug.Log($"JSON File: \n{json}");
        }

        public void ButtonSave()
        {
            Serialize();
            DeSerialize();
        }
    }
}
