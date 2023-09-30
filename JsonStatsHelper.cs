using System.Text.Json;
using System.Collections.Generic;
using System.IO;

namespace gruppe_2_easyword
{
    public static class JsonHelper
    {
        public static void SaveDictionaryToJson<T>(Dictionary<string, T> dictionary, string path)
        {
            var json = JsonSerializer.Serialize(dictionary);
            File.WriteAllText(path, json);
        }

        public static Dictionary<string, T> LoadDictionaryFromJson<T>(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<Dictionary<string, T>>(json);
            }
            return new Dictionary<string, T>();
        }

    }
}
