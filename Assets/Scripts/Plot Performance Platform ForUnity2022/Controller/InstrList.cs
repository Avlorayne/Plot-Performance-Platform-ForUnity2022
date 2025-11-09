using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Plot_Performance_Platform_ForUnity2022.Instruction;
using UnityEngine;

namespace Plot_Performance_Platform_ForUnity2022.Controller
{
    public class InstrList
    {
        [SerializeField]
        private List<InstrParam> _instrParams = new List<InstrParam>();

        public InstrParam this[int index]
        {
            get => _instrParams[index];
            set => _instrParams[index] = value;
        }
        public string Serialize()
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[\n");
            foreach (var instrParam in _instrParams)
            {
                jsonBuilder.Append(InstrParam.Serialize(instrParam)+",\n");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 2, 2);
            jsonBuilder.Append("\n]\n");

            return jsonBuilder.ToString();
        }

        public void Deserialize(string jsonString)
        {
            _instrParams.Clear();

            // 添加输入验证
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Debug.LogError("JSON string is null or empty");
                return;
            }

            // 清理字符串：移除 BOM 和空白字符
            string cleanJson = jsonString.Trim('\uFEFF', ' ', '\t', '\n', '\r');
            Debug.Log(@$"Plot Json:
{cleanJson}");

            if (string.IsNullOrEmpty(cleanJson))
            {
                Debug.LogError("JSON string contains only whitespace");
                return;
            }

            // 验证 JSON 是否以数组开始
            if (!cleanJson.StartsWith("["))
            {
                Debug.LogError($"Invalid JSON format. Expected array, got: {cleanJson.Substring(0, Math.Min(50, cleanJson.Length))}...");
                return;
            }

            try
            {
                Debug.Log("Start to Deserialize:-------------------------------------------------");
                List<InstrParam> list = JsonSerializer.Deserialize<List<InstrParam>>(cleanJson);

                if (list == null)
                {
                    Debug.Log("Deserialization failed - null result");
                    return;
                }

                foreach (var instrParam in list)
                {
                    InstrParam convert = InstrParam.Convert(instrParam);
                    _instrParams.Add(convert);
                }

                Debug.Log($"Successfully deserialized {list.Count} items--------------------------");
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

        public void Print()
        {
            Debug.Log($"{this.GetType().Name}:------------------------------------------------");
            // foreach (var instParam in this)
            // {
            //     InstrParam.Print(instParam);
            // }
            for (int i = 0; i < _instrParams.Count; i++)
            {
                Debug.Log($"{i}: {InstrParam.PrintString(_instrParams[i])}");
            }
            Debug.Log($"{this.GetType().Name} In total {_instrParams.Count} items---------------------");
        }

        public List<InstrParam> Content => _instrParams;
    }
}
