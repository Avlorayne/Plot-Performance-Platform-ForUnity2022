using System;
using System.Text.Json;

namespace Knot.Include.Utility
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
