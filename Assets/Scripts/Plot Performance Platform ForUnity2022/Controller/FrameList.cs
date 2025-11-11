using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using Plot_Performance_Platform_ForUnity2022.Utility;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Controller
{
    public class FrameList
    {
        private const string DEVIDE_CHAR = "---------------------------------------------------------------------------";
        private const int MAX_PRINT = 65;

        private List<Frame> _frames = new();

        #region PropertyOverrider
        public Frame this[int index]
        {
            get => _frames[index];
            set => _frames[index] = value;
        }

        public List<Frame> Content => _frames;

        public void Add(Frame frame) => _frames.Add(frame);
        public void Add(InstrParam[] instructions) => _frames.Add(Frame.Create(instructions));
        public void Remove(int index) => _frames.RemoveAt(index);
        public void Clear() => _frames.Clear();
        public int Count => _frames.Count;
        #endregion

        #region Serialize
        public string Serialize()
        {
            if (_frames == null || _frames.Count == 0)
                return "[]";

            var frameJsonStrings = _frames.Select(frame => frame.Serialize());
            return JsonPrettyPrinter.Format("[" + string.Join(",", frameJsonStrings) + "]");
        }

        public void Deserialize(string jsonString)
        {
            _frames.Clear();

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Debug.LogError("JSON string is null or empty");
                return;
            }

            string cleanJson = jsonString.Trim('\uFEFF', ' ', '\t', '\n', '\r');
            Debug.Log($"Plot Json:\n{JsonPrettyPrinter.FormatWithColor(cleanJson)}");

            if (string.IsNullOrEmpty(cleanJson))
            {
                Debug.LogError("JSON string contains only whitespace");
                return;
            }

            if (!cleanJson.StartsWith("["))
            {
                Debug.LogError($"Invalid JSON format. Expected array, got: {cleanJson.Substring(0, Math.Min(50, cleanJson.Length))}...");
                return;
            }

            try
            {
                Debug.Log($"Start to Deserialize: {DEVIDE_CHAR}");

                // 反序列化为 InstrParam 数组的列表
                List<InstrParam[]> rawList = JsonSerializer.Deserialize<List<InstrParam[]>>(cleanJson);

                if (rawList == null)
                {
                    Debug.Log("Deserialization failed - null result");
                    return;
                }

                // 转换为 Frame 对象
                foreach (var instructionArray in rawList)
                {
                    Frame frame = new Frame();
                    foreach (var instr in instructionArray)
                    {
                        InstrParam convert = InstrParam.Convert(instr);
                        frame.Add(convert);
                    }
                    _frames.Add(frame);
                }

                Debug.Log($"Successfully deserialized {rawList.Count} items {DEVIDE_CHAR}".Truncate(MAX_PRINT));
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON deserialization error: {ex.Message}");
                Debug.LogError($"JSON content: {cleanJson}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error during deserialization: {ex.Message}");
            }
        }
        #endregion

        #region Print
        public void Print()
        {
            Debug.Log($"{nameof(FrameList)}: {DEVIDE_CHAR}".Truncate(MAX_PRINT));

            for (int i = 0; i < _frames.Count; i++)
            {
                Debug.Log($"Frame {i}: \n{_frames[i].PrintString()}");
            }

            Debug.Log($"{nameof(FrameList)} In total {_frames.Count} items {DEVIDE_CHAR}".Truncate(MAX_PRINT));
        }
        #endregion
    }
}
