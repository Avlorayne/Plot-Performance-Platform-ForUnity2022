using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plot_Performance_Platform_ForUnity2022.Construct;
using Plot_Performance_Platform_ForUnity2022.Utility;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.src.DataSequence
{
    [Serializable]
    public class Frame
    {
        [JsonInclude] private List<InstrParam> _instructions = new List<InstrParam>();

        #region Properties and Methods
        [JsonIgnore] public List<InstrParam> Content => _instructions;
        [JsonIgnore] public int Count => _instructions.Count;

        public InstrParam this[int index]
        {
            get => _instructions[index];
            set => _instructions[index] = value;
        }

        public void Add(InstrParam instruction) => _instructions.Add(instruction);
        public void AddRange(IEnumerable<InstrParam> instructions) => _instructions.AddRange(instructions);
        public void Remove(InstrParam instruction) => _instructions.Remove(instruction);
        public void RemoveAt(int index) => _instructions.RemoveAt(index);
        public void Clear() => _instructions.Clear();
        public bool Contains(InstrParam instruction) => _instructions.Contains(instruction);
        #endregion

        #region Serialization
        public string Serialize()
        {
            string json = JsonSerializer.Serialize(this);
            string replaced = JsonSerializer.Serialize(_instructions);

            string replacing;

            if (_instructions == null || _instructions.Count == 0)
            {
                replacing =  "[]";
            }
            else
            {
                var instructionJsonStrings = _instructions.Select(instr => InstrParam.Serialize(instr));
                replacing =  "[" + string.Join(",", instructionJsonStrings) + "]";
            }
            Debug.Log(
                @$"[Frame.Serialize]
jsonStrng: {JsonPrettyPrinter.Format(json)}
replaced: {JsonPrettyPrinter.Format(replaced)}
replacing: {JsonPrettyPrinter.Format(replacing)}");

            string result = json.Replace(replaced, replacing);

            Debug.Log($"[Frame.Serialize]result: {JsonPrettyPrinter.Format(result)}");

            return result;
        }

        public void Deserialize(string jsonString)
        {
            _instructions.Clear();

            if (string.IsNullOrWhiteSpace(jsonString))
                return;

            string cleanJson = jsonString.Trim('\uFEFF', ' ', '\t', '\n', '\r');

            try
            {
                var instructions = System.Text.Json.JsonSerializer.Deserialize<List<InstrParam>>(cleanJson);
                if (instructions != null)
                {
                    foreach (var instr in instructions)
                    {
                        InstrParam converted = InstrParam.Convert(instr);
                        _instructions.Add(converted);
                    }
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                Debug.LogError($"[Frame.Deserialize]Frame deserialization error: {ex.Message}");
            }
        }
        #endregion

        #region Print
        public string PrintString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Frame with {_instructions.Count} instructions:");

            for (int i = 0; i < _instructions.Count; i++)
            {
                sb.AppendLine($"  Instruction {i}: {InstrParam.PrintString(_instructions[i])}");
            }

            return sb.ToString();
        }

        public void Print()
        {
            Debug.Log($"[Frame]{PrintString()}");
        }
        #endregion

        #region Factory Methods
        public static Frame Create(params InstrParam[] instructions)
        {
            Frame frame = new Frame();
            frame.AddRange(instructions);
            return frame;
        }

        public static Frame FromArray(InstrParam[] instructions)
        {
            return Create(instructions);
        }
        #endregion
    }
}
