using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using WPFLabel = System.Windows.Controls.Label;

namespace gruppe_2_easyword
{
    public  class WordManagement
    {
        public void SetNextWord(List<string> currentRoundWords, List<string> nextRoundWords, ref int currentIndex, ref string currentWord, bool translateToY, TextBlock wordToTransTextBlock)
        {
            if (currentRoundWords.Count == 0)
            {
                if (nextRoundWords.Count == 0)
                {
                    MessageBox.Show("Alle Wörter wurden korrekt übersetzt!");
                    return;
                }

                // Beginnen Sie die nächste Runde
                currentRoundWords = new List<string>(nextRoundWords);
                nextRoundWords.Clear();
            }

            // Wählen Sie zufällig ein Wort aus der aktuellen Runde
            Random random = new Random();
            currentIndex = random.Next(0, currentRoundWords.Count);

            currentWord = currentRoundWords[currentIndex].Split(';')[translateToY ? 0 : 1];
            wordToTransTextBlock.Text = currentWord; // Hier wurde die Änderung vorgenommen
        }

        public void SetLanguageLabels(bool isGermanToEnglish, WPFLabel label1)
        {
            if (isGermanToEnglish)
            {
                // Von Englisch nach Deutsch
                label1.Content = "Wort übersetztn auf Englisch"; // Hier wurde die Änderung vorgenommen
            }
            else
            {
                // Von Deutsch nach Englisch
                label1.Content = "Wort übersetztn auf Deutsch"; // Und hier
            }
        }

    }

}
