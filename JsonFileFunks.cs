using System;
using System.Collections.Generic;
using System.IO;

namespace gruppe_2_easyword
{
    public static class FileManagement
    {
        private static string jsonFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "files.json");

        public static List<string> LoadFilesFromJson()
        {
            if (File.Exists(jsonFilePath))
            {
                string json = File.ReadAllText(jsonFilePath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
            }
            return new List<string>();
        }



        public static void SaveFilesToJson(List<string> files)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(files);
            File.WriteAllText(jsonFilePath, json);
        }



        public static void RemoveFile(string fileToRemove, List<string> files)
        {
            files.Remove(fileToRemove);
            SaveFilesToJson(files);
        }


        public static void AddFile(string filePath, List<string> files)
        {
            files.Add(filePath);
            SaveFilesToJson(files);
        }
    }

}
