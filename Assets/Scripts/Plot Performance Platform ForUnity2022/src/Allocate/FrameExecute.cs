using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Unity.VisualScripting;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.src.Allocate
{
    public enum ExState
    {
        Null,
        Ready,
        Executing,
        Completed,
        End
    }

    public class FrameExecute
    {
        private KeyValuePair<InstrParam, InstrExecute>[] InstrsPairs { get; }

        public Dictionary<Type, KeyValuePair<InstrParam, InstrExecute>[]>
            ExeTable { get; }

        public bool IsCanBeSkipped { get; } = true;
        public ExState ExState { get; set; } = ExState.Null;

        private int CompletedExes;

        #region Property Override

        public KeyValuePair<InstrParam, InstrExecute>[]
            this[Type type] { get => ExeTable[type]; }

        public KeyValuePair<InstrParam, GameObject>[] Pairs { get; private set; }

        public GameObject[] Executors { get; private set; }


        public int Count => InstrsPairs.Length;

        #endregion

        #region Construction
        public FrameExecute(KeyValuePair<InstrParam, GameObject>[] pairs)
        {
            KeyValuePair<InstrParam, GameObject>[] KeyValuePairs = pairs;

            CompletedExes = 0;

            #region Check CoExist Valid
            List<InstrParam> cantCoExist = new List<InstrParam>();

            foreach (var pair in pairs)
            {
                if (pair.Key.IsRelese == false)
                {
                    IsCanBeSkipped = false;
                    break;
                }
            }

            foreach (var pair in pairs)
            {
                if (pair.Key.IsCanCoexist == false)
                {
                    if(!cantCoExist.Contains(pair.Key))
                    {
                        cantCoExist.Add(pair.Key);
                    }
                    else
                    {
                        Debug.LogError($"[FrameExecute.Construction]{pair.Key.Name} can't be coexist in the same frame.");
                    }
                }
            }

            #endregion

            #region Construct InstrsPairs

            Pairs = KeyValuePairs;
            Executors = KeyValuePairs.Select(x => x.Value).ToArray();

            List<KeyValuePair<InstrParam, InstrExecute>> pairList = new ();
            foreach (var pair in KeyValuePairs)
            {
                InstrParam param = pair.Key;
                InstrExecute execute = pair.Value.GetComponents<MonoBehaviour>().Select(mono =>
                {
                    if(mono.GetType().IsSubclassOf(typeof(InstrExecute)))
                        return mono;

                    return null;
                }).FirstOrDefault() as InstrExecute;

                pairList.Add(new KeyValuePair<InstrParam, InstrExecute>(param, execute));
            }

            InstrsPairs = pairList.ToArray();

            #endregion

            #region Construct ExeTable

            ExeTable = InstrsPairs.
                GroupBy(x => x.Key.GetType()).
                ToDictionary(g => g.Key, g => g.ToArray());

            #endregion

            ExState = ExState.Ready;
        }
        #endregion

        #region Execute

        public IEnumerator CoSubExecute(KeyValuePair<InstrParam, InstrExecute>[] pairs)
        {
            #region Regist Events
            foreach (var pair in pairs)
            {
                pair.Value.OnExecuting += OnInstrExecuting;
                pair.Value.OnCompleted += OnInstrCompleted;
            }
            #endregion

            #region Start Execute
            foreach (var pair in pairs)
            {
                InstrExecute execute = pair.Value;
                InstrExecute[] exeSeq = pairs.Select(p => p.Value).ToArray();

                if (execute == exeSeq[0])
                {
                    if(execute.Param == null)
                        execute.Init(pair.Key);

                    if(execute.ExState == ExState.Ready)
                        execute.Execute();
                }
                else
                    for (int i = 1; i < exeSeq.Length; i++)
                    {
                        if (execute == exeSeq[i])
                        {
                            if (exeSeq[i - 1].Param.IsBeWaited && exeSeq[i - 1].ExState == ExState.Executing)
                            {
                                yield return new WaitUntil(() => exeSeq[i - 1].ExState == ExState.Completed);
                            }

                            if(execute.Param == null)
                                execute.Init(pair.Key);

                            if(execute.ExState == ExState.Ready)
                                execute.Execute();
                        }
                    }
            }
            #endregion
        }

        public void Skip()
        {
            if (!IsCanBeSkipped)
            {
                return;
            }

            InstrExecute[] exeSeq = InstrsPairs.Select(pair => pair.Value).ToArray();
            foreach (var execute in exeSeq)
            {
                execute.Interrupt();
            }
        }

        public void End()
        {
            InstrExecute[] exeSeq = InstrsPairs.Select(pair => pair.Value).ToArray();

            if (ExState != ExState.Completed)
            {
                return;
            }

            foreach (var execute in exeSeq)
            {

                execute.End();
            }

            #region UnRegister Events
            if (InstrsPairs == null)
                return;
            foreach (var pair in InstrsPairs)
            {
                pair.Value.OnExecuting -= OnInstrExecuting;
                pair.Value.OnCompleted -= OnInstrCompleted;
            }
            #endregion
        }

        #endregion

        #region Delegate And Event
        // public delegate void ExecuteHandler();
        // public event ExecuteHandler OnCompleted;

        private void OnInstrExecuting()
        {
            if (ExState == ExState.Ready)
            {
                ExState = ExState.Executing;
            }
        }

        private void OnInstrCompleted()
        {
            CompletedExes++;
            if (CompletedExes == Count)
            {
                ExState = ExState.Completed;
                // this.OnCompleted?.Invoke();
            }
        }

        #endregion

        #region Print
        public string PrintString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Frame with {Pairs.Length} instructions:");

            foreach (var pair in Pairs)
            {
                sb.AppendLine($"\tExecutor: {pair.Value.GetType().Name}\nInstruction: {InstrParam.PrintString(pair.Key)}");
            }

            return sb.ToString();
        }
        #endregion
    }
}
