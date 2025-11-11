using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Controller
{
    [Serializable]
    public class Frame
    {
        private List<InstrParam> _instructions = new List<InstrParam>();

        #region Properties and Methods
        public List<InstrParam> Instructions => _instructions;

        public int Count => _instructions.Count;

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
            if (_instructions == null || _instructions.Count == 0)
                return "[]";

            var instructionJsonStrings = _instructions.Select(InstrParam.Serialize);
            return "[" + string.Join(",", instructionJsonStrings) + "]";
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
                Debug.LogError($"Frame deserialization error: {ex.Message}");
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
            Debug.Log(PrintString());
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
