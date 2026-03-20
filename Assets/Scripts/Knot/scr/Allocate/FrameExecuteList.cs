using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Include.Construct;
using Knot.scr.Controller;
using Knot.scr.DataSequence;
using Knot.Include.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Knot.scr.Allocate
{
    public class FrameExecuteList
    {
        #region Const Char
        private const string DEVIDE_CHAR = "---------------------------------------------------------------------------";
        private static int MaxPrint { get; } = 65;
        #endregion

        private const string Tag = "Instruction";


        /// the Final Data needed by Controller
        private readonly List<FrameExecute> _frameExList = new();
        /// <summary>
        /// InstrExecute Type - Prefab Type
        /// </summary>
        /// <key>InstrExecute Type</key>
        /// <Value>Prefab</Value>
        private readonly Dictionary<Type,GameObject> _prefabTable = new();
        private readonly Dictionary<Type, ExecutePool> _exePoolTable = new();

        #region Property Override

        public FrameExecute this[int index] => _frameExList[index];
        public int Count => _frameExList.Count;
        public List<FrameExecute> Content => _frameExList;
        private HashSet<FrameExecute> _hashSet = new();
        public bool Contains(FrameExecute frameExecute)
        {
            if (_hashSet.Count != _frameExList.Count)
            {
                _hashSet.Clear();
                _hashSet = new HashSet<FrameExecute>(_frameExList);
            }

            return _hashSet.Contains(frameExecute);
        }
        
        #endregion

        #region Construction

        public FrameExecuteList(FrameList frames)
        {
            Cleanup();

            GetAllPrefabs();

            foreach (var frame in frames.Content)
            {
                FrameExecute allocatedFrame = Allocate(frame);
                _frameExList.Add(allocatedFrame);
            }

            foreach (var pool in _exePoolTable)
            {
                pool.Value.SetActive(false);
            }
        }

        // Clean up All Data in this Class
        private void Cleanup()
        {
            // 清理对象池
            foreach (var pool in _exePoolTable.Values)
            {
                pool.Clear();
            }
            _exePoolTable.Clear();

            // 清理执行器列表（对象池已经清理了这些对象，所以只需要清空列表）
            _frameExList.Clear();
        }

        /// <summary>
        /// Load all Prefabs from assets
        /// </summary>
        private void GetAllPrefabs()
        {
            // 查找所有预制体资源的GUID
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (var guid in prefabGuids)
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
                        Debug.LogError($"[FrameExecuteList.GetAllPrefabs]{path}: {prefab} does not contain Script!");
                        continue;
                    }

                    // Check MonoBehaviour Type
                    List<Type> targetTypes = new();
                    foreach (var script in scripts)
                    {
                        Type scriptType = script.GetType();
                        if (scriptType.IsSubclassOf(typeof(InstrExecute)))
                        {
                            targetTypes.Add(scriptType);
                        }
                    }
                    // no executor attached
                    if (targetTypes.Count == 0)
                    {
                        Debug.LogError($"[FrameExecuteList.GetAllPrefabs]{path}: {prefab} does not contain InstrExecute Script!");
                        continue;
                    }

                    // more than one executor attached
                    if (targetTypes.Count != 1)
                    {
                        Debug.LogError($"[FrameExecuteList.GetAllPrefabs]{path}: {prefab} Contains more than one InstrExecute Script!");
                        continue;
                    }

                    Type type = targetTypes[0];

                    // there exists the same type, so this Executor is excess
                    if (!_prefabTable.TryAdd(type, prefab))
                    {
                        Debug.LogError($"[FrameExecuteList.GetAllPrefabs]{type} Matches multiple Executors!");
                        continue;
                    }

                    Debug.Log($"[FrameExecuteList.GetAllPrefabs]Find instr Prefab: {path}: {type.Name}");
                }
            }

            Debug.Log($"[FrameExecuteList.GetAllPrefabs]Found {_prefabTable.Count} prefabs in total");
        }

        /// <summary>
        /// Allocate by each frame.
        /// At the same frame, several executors of the same kind might be requested.
        /// the pool provide several executors, and they'll be return after the frame if released.
        /// </summary>
        /// <param name="frame"></param>
        private FrameExecute Allocate(Frame frame)
        {
            List<KeyValuePair<InstrParam, InstrExecute>> exeTable = new();

            // allocate each executor to the matched param
            foreach (var param in frame.Content)
            {
                InstrExecute execute = GetReleasedExecutor(param);

                if (execute != null)
                {
                    exeTable.Add(new KeyValuePair<InstrParam, InstrExecute>(param, execute));
                }
                else
                {
                    throw new InvalidOperationException($"[FrameExecuteList.Allocate] Failed to create or find executor for {param.ExecutorType}");
                }
            }

            var frameExe = new FrameExecute(exeTable.ToArray());

            // return those released to pool
            foreach (var pair in exeTable)
            {
                if (pair.Key.IsRelease)
                {
                    Type exeType = pair.Key.ExecutorType;
                    ExecutePool pool = _exePoolTable[exeType];
                    pool.Release(pair.Value);
                }
            }

            Debug.Log($"[FrameExecuteList.Allocate] FrameExecute Created : \n{string.Join("\n",frameExe.InstrsPairs.Select(p => $"{p.Key.GetType()}: { p.Value.GetType()}"))}");

            return frameExe;
        }

        private InstrExecute GetReleasedExecutor(InstrParam param)
        {
            Type exeType = param.ExecutorType;
            if (exeType == null)
            {
                throw new InvalidOperationException($"[FrameExecuteList.GetReleasedExecutor] ExecutorType is null for param: {param.Name}");
            }

            // Step1. 获取或创建对应类型的对象池
            if (!_exePoolTable.TryGetValue(exeType, out var pool))
            {
                if (!_prefabTable.TryGetValue(exeType, out var prefab))
                {
                    throw new InvalidOperationException($"[FrameExecuteList.GetReleasedExecutor] Failed to find prefab for executor type: {exeType}");
                }
                
                pool = new ExecutePool(prefab, exeType.ToString());
                _exePoolTable.Add(exeType, pool);

                Debug.Log($"[FrameExecuteList.GetReleasedExecutor] new pool for {exeType} Created: {exeType.Name}");
            }

            // Step2. 从对象池获取可用的执行器
            InstrExecute executor = pool.Get();

            return executor;
        }
        #endregion

        #region Print
        public void Print()
        {
            Debug.Log($"[{nameof(FrameExecuteList)}] {DEVIDE_CHAR}".Truncate(MaxPrint));

            for (int i = 0; i < _frameExList.Count; i++)
            {
                Debug.Log($"Frame {i}: \n{_frameExList[i].PrintString()}");
            }

            Debug.Log($"[{nameof(FrameExecuteList)}] In total {_frameExList.Count} items {DEVIDE_CHAR}".Truncate(MaxPrint));
        }
        #endregion
    }
}
