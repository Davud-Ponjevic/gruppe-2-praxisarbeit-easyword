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
            private Dictionary<string, List<string>> x_to_y = new Dictionary<string, List<string>>();
            private Dictionary<string, List<string>> y_to_x = new Dictionary<string, List<string>>();

            public Translator(string[] data)
            {
                foreach (string entry in data)
                {
                    string[] parts = entry.Split(';');
                    string x = parts[0];
                    string y = parts[1];

                    if (!x_to_y.ContainsKey(x))
                        x_to_y[x] = new List<string>();
                    x_to_y[x].Add(y);

                    if (!y_to_x.ContainsKey(y))
                        y_to_x[y] = new List<string>();
                    y_to_x[y].Add(x);
                }
            }

        private string Check(string word, Dictionary<string, List<string>> wordDict, string userTranslation)
        {
            // Konvertieren Sie das Wort und die Benutzerübersetzung in Kleinbuchstaben
            string lowerCaseWord = word.ToLower();
            string lowerCaseTranslation = userTranslation.ToLower();

            // Überprüfen Sie, ob das Wort im Wörterbuch vorhanden ist
            if (!wordDict.ContainsKey(lowerCaseWord))
            {
                return $"Das Wort {word} wurde nicht im Wörterbuch gefunden.";
            }

            string translations = string.Join(";", wordDict[lowerCaseWord]);

            if (wordDict[lowerCaseWord].Any(t => t.Equals(lowerCaseTranslation, StringComparison.OrdinalIgnoreCase)))
                return $"Richtig! Mögliche Übersetzungen von {word} sind: {translations}";
            else
                return $"Falsch! Mögliche Übersetzungen von {word} sind: {translations}";
        }




        public string CheckXToY(string word, string userTranslation)
            {
                return Check(word, x_to_y, userTranslation);
            }

            public string CheckYToX(string word, string userTranslation)
            {
                return Check(word, y_to_x, userTranslation);
            }
        }
    

}
