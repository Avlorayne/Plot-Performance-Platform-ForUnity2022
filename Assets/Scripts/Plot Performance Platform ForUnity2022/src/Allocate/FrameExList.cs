using System;
using System.Collections.Generic;
using System.Linq;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Plot_Performance_Platform_ForUnity2022.Include.Utility;
using Plot_Performance_Platform_ForUnity2022.src.DataSequence;
using UnityEngine;
using UnityEditor;

namespace Plot_Performance_Platform_ForUnity2022.src.Allocate
{

    public class FrameExecuteList
    {
        #region Const Char
        private const string DEVIDE_CHAR = "---------------------------------------------------------------------------";
        public static int MaxPrint { get; } = 65;
        #endregion

        private const string Tag = "Instruction";

        private List<FrameExecute> frameExList = new();

        /// <summary>
        /// InstrExecute Type - Prefab Type
        /// </summary>
        /// <key>InstrExecute Type</key>
        /// <Value>Prefab</Value>
        private Dictionary<Type,GameObject> prefabDict = new();

        private Dictionary<InstrParam, GameObject> ExecutorDict = new();

        private List<GameObject> Executors = new();



        #region Construction

        public FrameExecuteList(FrameList frames)
        {
            Cleanup();

            GetAllPerfabs();

            foreach (var frame in frames.Content)
            {
                foreach (var instr in frame.Content)
                {
                    Allocate(instr);
                }
            }

            // 获取所有唯一的值
            Executors = ExecutorDict.Select(pair => pair.Value).Distinct().ToList();

            // 按帧划分
            DevideByFrames(frames);
        }

        public void Cleanup()
        {
            foreach (var executor in Executors)
            {
                if (executor != null)
                {
                    GameObject.DestroyImmediate(executor);
                }
            }
            Executors.Clear();
            ExecutorDict.Clear();
            frameExList.Clear();
        }

        private void GetAllPerfabs()
        {
            // 查找所有预制体资源的GUID
            string[] PrefabGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (var guid in PrefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                // Check Tag
                if (prefab != null && prefab.CompareTag(Tag))
                {
                    // could find diverse scripts
                    MonoBehaviour[] scripts = prefab.GetComponents<MonoBehaviour>();
                    if (scripts.Length == 0)
                    {
                        Debug.LogError($"[FrameExecuteList.GetAllPerfabs]{path}: {prefab} does not contain Script!");
                        continue;
                    }

                    // Check MonoBehaviour Type
                    Type targetType = null;
                    foreach (var script in scripts)
                    {
                        Type sriptType = script.GetType();
                        if (sriptType.IsSubclassOf(typeof(InstrExecute)))
                        {
                            targetType = sriptType;
                            break;
                        }
                    }

                    if (targetType == null)
                    {
                        Debug.LogError($"[FrameExecuteList.GetAllPerfabs]{path}: {prefab} does not contain InstrExecute Script!");
                        continue;
                    }

                    if (prefabDict.ContainsKey(targetType))
                    {
                        Debug.LogError($"[FrameExecuteList.GetAllPerfabs]{targetType} Matches multiple Executors!");
                        continue;
                    }

                    prefabDict.Add(targetType, prefab);

                    Debug.Log($"[FrameExecuteList.GetAllPerfabs]Find instr Prefab: {path}: {targetType.Name}");
                }
            }

            Debug.Log($"[FrameExecuteList.GetAllPerfabs]Found {prefabDict.Count} prefabs in total");
        }

        private void Allocate(InstrParam param)
        {
            GameObject execute = GetReleasedExecutor(param);

            // Add null check before adding to dictionary
            if (execute != null)
            {
                ExecutorDict.Add(param, execute);
            }
            else
            {
                throw new InvalidOperationException($"[FrameExecuteList.Allocate]Failed to create or find executor for {param.ExecutorType}");
            }
        }

        private GameObject GetReleasedExecutor(InstrParam param)
        {
            // Step1. Find Type-Match Pairs used to exist
            List<KeyValuePair<InstrParam, GameObject>> matchPairs = new();

            foreach (var frameEx in frameExList)
            {
                KeyValuePair<InstrParam, GameObject>[] pairs = frameEx.Content
                    .Where(pair => param.ExecutorType == pair.Value.GetType())
                    .ToArray();

                matchPairs.AddRange(pairs);
            }
            Debug.Log($"[FrameExecuteList.GetReleasedExecutor]Macthed Pairs: {matchPairs.Count}");

            // Step2. Get Released Executor
            List<GameObject> releasedExecutors = new ();

            foreach (var pair in matchPairs)
            {
                if (pair.Key.IsRelese)
                {
                    releasedExecutors.Add(pair.Value);
                }
                else if (releasedExecutors.Contains(pair.Value)) // !pair.Key.IsRelese && releasedExecutors.Contains(pair.Value)
                {
                    releasedExecutors.Remove(pair.Value);
                }
            }

            // Step3. Check if there exists a usable executor.
            // if not, Create a new instance and return.
            if (releasedExecutors.Count == 0)
            {
                Type _executorType = param.ExecutorType;
                if (_executorType != null && prefabDict.TryGetValue(_executorType, out var prefabType))
                {
                    // 创建新对象
                    GameObject _object = GameObject.Instantiate(
                        prefabType,
                        new Vector3(),
                        Quaternion.identity);

                    if (_object != null)
                    {
                        releasedExecutors.Add(_object);
                    }
                    else
                    {
                        Debug.LogError($"[FrameExecuteList.GetReleasedExecutor]Failed to create instance of {param.ExecutorType}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"[FrameExecuteList.GetReleasedExecutor]Failed to Find Executor Type of {param.ExecutorType}");
                }
            }

            return releasedExecutors.FirstOrDefault();
        }

        public void DevideByFrames(FrameList frames)
        {
            frameExList.Clear();

            foreach (var frame in frames.Content)
            {
                List<KeyValuePair<InstrParam, GameObject>> pairs = new();

                foreach (var instr in frame.Content)
                {
                    if (ExecutorDict.TryGetValue(instr, out GameObject execute))
                    {
                        pairs.Add(new KeyValuePair<InstrParam, GameObject>(instr, execute));
                    }
                    else
                    {
                        Debug.LogError($"[FrameExecuteList.DevideByFrames]No executor found for instruction: {instr.Name}");
                    }
                }

                frameExList.Add(new FrameExecute(pairs.ToArray()));
            }
        }

        #endregion

        #region Execute



        #endregion

        #region Print
        public void Print()
        {
            Debug.Log($"[{nameof(FrameExecuteList)}] {DEVIDE_CHAR}".Truncate(MaxPrint));

            for (int i = 0; i < frameExList.Count; i++)
            {
                Debug.Log($"Frame {i}: \n{frameExList[i].PrintString()}");
            }

            Debug.Log($"[{nameof(FrameExecuteList)}] In total {frameExList.Count} items {DEVIDE_CHAR}".Truncate(MaxPrint));
        }
        #endregion
    }
}
