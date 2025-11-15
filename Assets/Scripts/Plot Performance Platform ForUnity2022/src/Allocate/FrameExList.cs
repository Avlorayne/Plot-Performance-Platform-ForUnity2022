using System;
using System.Collections.Generic;
using System.Linq;
using Plot_Performance_Platform_ForUnity2022.Construct;
using Plot_Performance_Platform_ForUnity2022.src.DataSequence;
using Plot_Performance_Platform_ForUnity2022.Utility;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.src.Allocate
{

    public class FrameExecuteList
    {
        #region Const Char
        private const string DEVIDE_CHAR = "---------------------------------------------------------------------------";
        public static int MaxPrint { get; } = 65;
        #endregion

        private List<FrameExecute> frameExList = new();

        private Dictionary<InstrParam, InstrExecute> InstrDict = new();
        private List<InstrExecute> Executors = new();

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

        #region Construction

        public FrameExecuteList(FrameList frames)
        {
            foreach (var frame in frames.Content)
            {
                foreach (var instr in frame.Content)
                {
                    Allocate(instr);
                }
            }

            // 获取所有唯一的值
            Executors = InstrDict.Select(pair => pair.Value).Distinct().ToList();

            // 按帧划分
            DevideByFrames(frames);
        }


        private void Allocate(InstrParam param)
        {
            InstrExecute execute = GetReleasedExecutor(param);

            if (execute == null)
            {
                Type type = param.ExecutorType;
                try
                {
                    execute = Activator.CreateInstance(type) as InstrExecute;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"[FrameExecuteList.Allocate]Failed to create instance of {type?.Name ?? "unknown type"}", ex);
                }
            }

            // Add null check before adding to dictionary
            if (execute != null)
            {
                InstrDict.Add(param, execute);
            }
            else
            {
                throw new InvalidOperationException($"[FrameExecuteList.Allocate]Failed to create or find executor for {param.ExecutorType}");
            }
        }


        private InstrExecute GetReleasedExecutor(InstrParam param)
        {
            // Part1. Find Type-Match Pairs
            List<KeyValuePair<InstrParam, InstrExecute>> matchPairs = new();

            foreach (var frameEx in frameExList)
            {
                KeyValuePair<InstrParam, InstrExecute>[] pairs = frameEx.Content
                    .Where(pair => param.ExecutorType == pair.Value.GetType())
                    .ToArray();

                matchPairs.AddRange(pairs);
            }
            Debug.Log($"[FrameExecuteList.GetReleasedExecutor]Macthed Pairs: {matchPairs.Count}");

            if (matchPairs.Count == 0)
            {
                return null;
            }

            // Part2. Get Released Executor
            List<InstrExecute> releasedExecutors = new ();

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
            // Part3. Check if there exists a usable executor.
            // if not, Create a new instance and return.
            if (releasedExecutors.Count == 0)
            {
                Type type = param.ExecutorType;
                if (type != null)
                {
                    object instance = Activator.CreateInstance(type);
                    releasedExecutors.Add(instance as InstrExecute);
                }
                else
                {
                    throw new InvalidOperationException($"[FrameExecuteList.GetReleasedExecutor]Failed to create instance of {param.ExecutorType}");
                }
            }

            return releasedExecutors.FirstOrDefault();
        }

        public void DevideByFrames(FrameList frames)
        {
            frameExList.Clear();

            foreach (var frame in frames.Content)
            {
                List<KeyValuePair<InstrParam, InstrExecute>> pairs = new();

                foreach (var instr in frame.Content)
                {
                    if (InstrDict.TryGetValue(instr, out InstrExecute execute))
                    {
                        pairs.Add(new KeyValuePair<InstrParam, InstrExecute>(instr, execute));
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
    }
}
