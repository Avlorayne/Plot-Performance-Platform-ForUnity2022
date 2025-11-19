using System;
using System.Collections.Generic;
using System.IO;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using Plot_Performance_Platform_ForUnity2022.src.Allocate;
using Plot_Performance_Platform_ForUnity2022.src.DataSequence;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Plot_Performance_Platform_ForUnity2022.src.Controller
{
    public class PlotPerformSys : MonoBehaviour
    {
        private static PlotPerformSys _instance;
        public static PlotPerformSys Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PlotPerformSys();
                return _instance;
            }
        }

        #region Data
        [Header("剧情脚本")]
        public TextAsset plotJson;

        private FrameExecuteList FrameExecuteList;
        #endregion

        [SerializeField] private int index;

        public bool isClosed = true;


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            Debug.Log("[PlotPerformSys.Awake]");

            testData();
            LoadData();

            index = 0;
            ControlFrame();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                ControlFrame();
            }
        }

        void ControlFrame()
        {
            if (index >= FrameExecuteList.Count)
            {
                ClosePlot();
                return;
            }

            var currentFrame = FrameExecuteList[index];
            Debug.Log($"[PlotPerformSys.ControlFrame] FrameExecuteList[{index}]:{(currentFrame  != null ? "is called" : "is Null")}.]");

            switch (currentFrame.ExState)
            {
                case ExState.Null:

                    currentFrame.Active();
                    ControlFrame();
                    break;

                case ExState.Ready:

                    Execute(currentFrame);
                    break;

                case ExState.Executing:
                    // 执行中：跳过
                    if (currentFrame.IsCanBeSkipped)
                    {
                        Skip(currentFrame);
                    }
                    break;

                case ExState.Completed:
                    // 完成：结束并进入下一帧
                    End(currentFrame);
                    index++;

                    if (index < FrameExecuteList.Count)
                    {
                        // 下一帧自动准备执行
                        var nextFrame = FrameExecuteList[index];
                        nextFrame.Active();

                        if (nextFrame.ExState == ExState.Ready)
                        {
                            Execute(nextFrame);
                        }
                    }
                    else
                    {
                        ClosePlot();
                    }
                    break;

                default:
                    Debug.LogWarning($"[PlotPerformSys]Unexpected frame state: {currentFrame.ExState}");
                    break;
            }
        }

        #region Execute

        public void Execute(FrameExecute frameExecute)
        {
            Dictionary<Type, KeyValuePair<InstrParam, InstrExecute>[]> exeTable
                = FrameExecuteList.Content.Contains(frameExecute) ? FrameExecuteList[index].ExeTable
                    : frameExecute.ExeTable;
            Debug.Log($"[PlotPerformSys.Execute]Executing {(FrameExecuteList.Content.Contains(frameExecute) ? index : " ")}...");

            foreach (var exes in exeTable)
            {
                StartCoroutine(frameExecute.CoSubExecute(exes.Value));
            }
        }

        public void Skip(FrameExecute frameExecute)
        {
            frameExecute.Skip();
        }

        public void End(FrameExecute frameExecute)
        {
            frameExecute.End();
        }


        public void ClosePlot()
        {
            // FrameExecute closeFrame = new FrameExecute(new KeyValuePair<InstrParam, GameObject>[]{});
            // Execute(closeFrame);
            isClosed = true;
            this.gameObject.SetActive(false);
        }

        #endregion


        #region Data

        void testData()
        {
            FrameList tmp = new FrameList();
            Frame F1 = new Frame();
            F1.Add(new TypeDialogueParam(new Dialogue("name1","sentence1111122222222222")));
            Frame F2 = new Frame();
            F2.Add(new TypeDialogueParam(new Dialogue("name2","sentence2222222222333333")));

            tmp.Add(F1);
            tmp.Add(F2);
            Serialize(tmp);
        }
        void LoadData()
        {
            FrameList frameList = DeSerialize();
            FrameExecuteList = new FrameExecuteList(frameList);
            FrameExecuteList.Print();
            for (int i = 0; i < FrameExecuteList.Count; i++)
            {
                if (FrameExecuteList[i] == null)
                {
                    Debug.LogWarning($"[PlotPerformSys.LoadData]FrameExecuteList[{i}] is null.");
                }
            }
            Debug.Log($"[PlotPerformSys.LoadData] Load Completed.");
        }

        FrameList DeSerialize()
        {
            FrameList frameList = new FrameList();
            string json = plotJson.text.Trim('\uFEFF');
            frameList.Deserialize(json);
            frameList.Print();

            return frameList;
        }

        void Serialize( FrameList frameList)
        {
            string json = frameList.Serialize();
            string path = AssetDatabase.GetAssetPath(plotJson);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        // public void ButtonSave()
        // {
        //     Serialize();
        //     DeSerialize();
        // }
        #endregion
    }
}
