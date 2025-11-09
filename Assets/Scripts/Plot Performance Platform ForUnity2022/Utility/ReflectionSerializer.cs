using System;
using System.Text.Json;

namespace Plot_Performance_Platform_ForUnity2022.Utility
{
    public interface ISerializer
    {
        string Serialize(object obj);
    }

    public class ReflectionSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            if (obj == null) return null;

            Type type = obj.GetType();

            return JsonSerializer.Serialize(obj, type, new JsonSerializerOptions{IncludeFields = true, WriteIndented = true});
        }
    }
}
