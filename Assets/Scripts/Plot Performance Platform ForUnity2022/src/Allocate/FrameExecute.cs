using System.Collections.Generic;
using System.Text;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;
using Plot_Performance_Platform_ForUnity2022.src.DataSequence;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.src.Allocate
{
    public class FrameExecute
    {
        public KeyValuePair<InstrParam, GameObject>[] KeyValuePairs { get; set; }

        public bool IsCanBeSkipped { get; set; } = true;
        public bool isFrameComplete { get; set; } = false;
        public bool isFrameSkipped { get; set; } = false;

        #region Property Override

        public GameObject this[InstrParam key]
        {
            get
            {
                foreach (var pair in KeyValuePairs)
                {
                    if (pair.Key == key)
                    {
                        return pair.Value;
                    }
                }

                return null;
            }
        }

        public KeyValuePair<InstrParam, GameObject> this[int index]
        {
            get => KeyValuePairs[index];
            set => KeyValuePairs[index] = value;
        }

        public KeyValuePair<InstrParam, GameObject>[] Content => KeyValuePairs;

        public int Count => KeyValuePairs.Length;

        #endregion

        #region Constructor
        public FrameExecute(KeyValuePair<InstrParam, GameObject>[] pairs)
        {
            KeyValuePairs = pairs;
            foreach (var pair in pairs)
            {
                if (pair.Key.IsRelese == false)
                {
                    IsCanBeSkipped = false;
                    break;
                }
            }
        }
        #endregion

        public void Execute()
        {

        }

        public void Skip()
        {

        }

        #region Print
        public string PrintString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Frame with {KeyValuePairs.Length} instructions:");

            foreach (var pair in KeyValuePairs)
            {
                sb.AppendLine($"\tExecutor: {pair.Value.GetType().Name}\nInstruction: {InstrParam.PrintString(pair.Key)}");
            }

            return sb.ToString();
        }
        #endregion
    }
}
