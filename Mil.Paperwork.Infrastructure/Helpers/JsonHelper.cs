using System.IO;
using System.Text.Json;

namespace Mil.Paperwork.Infrastructure.Helpers
{
    public static class JsonHelper
    {
        public static T? ReadJson<T>(string jsonContent)
        {
            var result = JsonSerializer.Deserialize<T>(jsonContent);
            return result;
        }

        public static string WriteJson<T>(T obj)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(obj, options);
        }
    }
}
