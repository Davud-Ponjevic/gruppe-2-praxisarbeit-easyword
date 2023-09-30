using System.Collections.Generic;
using System.Linq;


namespace gruppe_2_easyword
{
    public static class MistakeManagement
    {
        private static readonly string mistakesFilePath = "mistakes.txt";

        public static void SaveMistakesToFile(Dictionary<string, int> wordMistakes)
        {
            System.IO.File.WriteAllLines(mistakesFilePath, wordMistakes.Select(kvp => $"{kvp.Key};{kvp.Value}"));
        }

        public static Dictionary<string, int> LoadMistakesFromFile()
        {
            Dictionary<string, int> loadedMistakes = new Dictionary<string, int>();

            if (System.IO.File.Exists(mistakesFilePath))
            {
                foreach (var line in System.IO.File.ReadAllLines(mistakesFilePath))
                {
                    var parts = line.Split(';');
                    loadedMistakes[parts[0]] = int.Parse(parts[1]);
                }
            }

            return loadedMistakes;
        }
    }

}
