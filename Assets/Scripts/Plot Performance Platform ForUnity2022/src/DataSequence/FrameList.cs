using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plot_Performance_Platform_ForUnity2022.Construct;
using Plot_Performance_Platform_ForUnity2022.Utility;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.src.DataSequence
{
    public class FrameList
    {
        #region Const Char
        private const string DEVIDE_CHAR = "---------------------------------------------------------------------------";
        private const int MAX_PRINT = 65;
        #endregion

        [JsonInclude] private List<Frame> _frames = new();

        #region PropertyOverrider
        public Frame this[int index]
        {
            get => _frames[index];
            set => _frames[index] = value;
        }

        [JsonIgnore] public List<Frame> Content => _frames;

        public void Add(Frame frame) => _frames.Add(frame);
        public void Add(InstrParam[] instructions) => _frames.Add(Frame.Create(instructions));
        public void Remove(int index) => _frames.RemoveAt(index);
        public void Clear() => _frames.Clear();
        [JsonIgnore] public int Count => _frames.Count;
        #endregion

        #region Serialize
        public string Serialize()
        {
            string json = JsonSerializer.Serialize(this);
            string replaced = JsonSerializer.Serialize(_frames);

            string replacing;
            if (_frames == null || _frames.Count == 0)
            {
                replacing =  "[]";
            }
            else
            {
                var frameJsonStrings = _frames.Select(frame => frame.Serialize());
                replacing = "[" + string.Join(",", frameJsonStrings) + "]";
            }
            Debug.Log(
                @$"[FrameList.Serialize]
jsonStrng: {JsonPrettyPrinter.Format(json)}
replaced: {JsonPrettyPrinter.Format(replaced)}
replacing: {JsonPrettyPrinter.Format(replacing)}");

            string result = json.Replace(replaced, replacing);

            Debug.Log($"[FrameList.Serialize]result: {JsonPrettyPrinter.Format(result)}");

            return JsonPrettyPrinter.Format(result);
        }

        public void Deserialize(string jsonString)
        {
            _frames.Clear();
            FrameList rawFrameList = new();
            // ------------------------------------------------
            #region JSON Check
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Debug.LogError("[FrameList.Deserialize]JSON string is null or empty");
                return;
            }

            string cleanJson = jsonString.Trim('\uFEFF', ' ', '\t', '\n', '\r');
            Debug.Log($"[FrameList.Deserialize]Plot Json:\n{JsonPrettyPrinter.FormatWithColor(cleanJson)}");

            if (string.IsNullOrEmpty(cleanJson))
            {
                Debug.LogError("[FrameList.Deserialize]JSON string contains only whitespace");
                return;
            }
            #endregion
            // ------------------------------------------
            #region JSON Parse
            try
            {
                Debug.Log($"Start to Deserialize: {DEVIDE_CHAR}");

                // 反序列化为 InstrParam 数组的列表
                rawFrameList = JsonSerializer.Deserialize<FrameList>(cleanJson);

                if (rawFrameList == null)
                {
                    Debug.Log("[FrameList.Deserialize] failed - null result");
                    return;
                }
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[FrameList.Deserialize]JSON deserialization error: {ex.Message}");
                Debug.LogError($"[FrameList.Deserialize]JSON content: {cleanJson}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FrameList.Deserialize]Unexpected error during deserialization: {ex.Message}");
            }
            #endregion
            // -------------------------------------------------
            #region Convert to frames
            try
            {
                // 转换为 Frame 对象
                foreach (var rawFrame in rawFrameList!.Content)
                {
                    Frame frame = new Frame();
                    foreach (var instr in rawFrame.Content)
                    {
                        InstrParam convert = InstrParam.Convert(instr);
                        frame.Add(convert);
                    }
                    _frames.Add(frame);
                }

                Debug.Log($"[FrameList.Deserialize]Successfully deserialized {rawFrameList.Count} items {DEVIDE_CHAR}".Truncate(MAX_PRINT));

            }
            catch (Exception ex)
            {
                Debug.LogError($"[FrameList.Deserialize]Unexpected error during deserialization:\n{ex.Message}");
            }
            #endregion
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
