using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace gruppe_2_easyword
{
    public class Translator
    {
        private Dictionary<string, List<string>> de_to_eng = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> eng_to_de = new Dictionary<string, List<string>>();

        public Translator(string[] data)
        {
            foreach (string entry in data)
            {
                string[] parts = entry.Split(';');
                string x = parts[0];
                string y = parts[1];

                if (!de_to_eng.ContainsKey(x))
                    de_to_eng[x] = new List<string>();
                de_to_eng[x].Add(y);

                if (!eng_to_de.ContainsKey(y))
                    eng_to_de[y] = new List<string>();
                eng_to_de[y].Add(x);
            }
        }

        private string Check(string word, Dictionary<string, List<string>> wordDict, string userTranslation)
        {
            string translations = string.Join(";", wordDict[word]);

            if (wordDict[word].Contains(userTranslation))
                return $"Richtig! Mögliche Übersetzungen von {word} sind: {translations}";
            else
                return $"Falsch! Mögliche Übersetzungen von {word} sind: {translations}";
        }


        public void CheckdeToeng(string word)
        {
            Check(word, de_to_eng);
        }

        public void CheckYToX(string word)
        {
            Check(word, eng_to_de);
        }
    }
}
