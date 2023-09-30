using System.Collections.Generic;


namespace gruppe_2_easyword
{
    public class StatisticsManager
    {
        private const string StatisticFilePath = "statistics.json";
        private Dictionary<string, int> wordMistakes;

        public StatisticsManager()
        {
            wordMistakes = JsonHelper.LoadDictionaryFromJson<int>(StatisticFilePath);
        }

        public void Save()
        {
            JsonHelper.SaveDictionaryToJson(wordMistakes, StatisticFilePath);
        }

        public Dictionary<string, int> GetStatistics()
        {
            return wordMistakes;
        }

        public void UpdateStatistics(string word)
        {
            // Hier können Sie Ihre Logik zum Aktualisieren der Statistik hinzufügen.
            // Zum Beispiel:
            if (wordMistakes.ContainsKey(word))
            {
                wordMistakes[word]++;
            }
            else
            {
                wordMistakes[word] = 1;
            }
        }

        public void Reset()
        {
            wordMistakes.Clear();
        }
    }

}
