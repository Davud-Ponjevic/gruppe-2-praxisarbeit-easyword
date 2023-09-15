using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using gruppe_2_easyword;
using Microsoft.Win32;
using System.IO;


namespace Transalto
{
    public partial class MainWindow : Window
    {
        private Translator translator;
        private string currentWord;
        private string[] data;
        private int currentIndex = 0;  // Index des aktuellen Worts
        private bool translateToY = true;  // Übersetzungsrichtung (true: X nach Y, false: Y nach X)

        // In Ihrer MainWindow Klasse:

        // Eine Liste von noch nicht korrekt übersetzten Wörtern
        private List<string> currentRoundWords;
        private List<string> nextRoundWords;


        // für lables 
        private bool isEnglishToGerman = true;



        public MainWindow()
        {
            InitializeComponent();
            SetLanguageLabels();

            // Open the file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "CSV-Datei auswählen";
            openFileDialog.Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                data = File.ReadAllLines(selectedFilePath);

                // Überprüfen Sie das Format jeder Zeile
                // Überprüfen Sie das Format jeder Zeile
                foreach (var line in data)
                {
                    var entries = line.Split(';');
                    if (entries.Length != 2 || string.IsNullOrWhiteSpace(entries[0]) || string.IsNullOrWhiteSpace(entries[1]))
                    {
                        MessageBox.Show($"Fehler im Format der Zeile: {line}. Jede Zeile sollte ein deutsches Wort und eine englische Übersetzung haben, getrennt durch ein Semikolon.");
                        this.Close();
                        return;  // Beenden Sie die Verarbeitung, wenn ein Fehler gefunden wird
                    }
                }

                

                translator = new Translator(data);

                // Initialisieren Sie die Listen für die aktuelle Runde und die nächste Runde
                currentRoundWords = new List<string>(data);
                nextRoundWords = new List<string>();

                SetNextWord();
            }
            else
            {
                // Optional: Schließen Sie die Anwendung, wenn keine Datei ausgewählt wurde
                this.Close();
            }
        }




        private void SetNextWord()
        {
            // Wenn alle Wörter der aktuellen Runde abgefragt wurden
            if (currentRoundWords.Count == 0)
            {
                if (nextRoundWords.Count == 0)
                {
                    MessageBox.Show("Alle Wörter wurden korrekt übersetzt!");
                    this.Close();
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
            WordToTrans.Text = currentWord;
        }

        private void TestResu(object sender, RoutedEventArgs e)
        {
            string userTranslation = WordTransResu.Text;
            string result;

            if (translateToY)
            {
                result = translator.CheckXToY(currentWord, userTranslation);  // Für Deutsch nach Englisch
            }
            else
            {
                result = translator.CheckYToX(currentWord, userTranslation);  // Für Englisch nach Deutsch
            }

            MessageBox.Show(result);
            WordTransResu.Text = "";

            // Wenn die Übersetzung falsch war, fügen Sie das Wort zur Liste für die nächste Runde hinzu
            if (result.StartsWith("Falsch"))
            {
                nextRoundWords.Add(currentRoundWords[currentIndex]);
            }

            // Entfernen Sie das aktuelle Wort aus der Liste für die aktuelle Runde
            currentRoundWords.RemoveAt(currentIndex);

            SetNextWord();  // Wechseln Sie das Wort nach dem Überprüfen der Übersetzung
        }

        private void Switchlangu(object sender, RoutedEventArgs e)
        {
            translateToY = !translateToY;  // Übersetzungsrichtung umschalten
            SetNextWord();
            isEnglishToGerman = !isEnglishToGerman;
            SetLanguageLabels();
            
        }
        
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {



        }
        private void SetLanguageLabels()
        {
            if (isEnglishToGerman)
            {
                // Von Englisch nach Deutsch
                Label1.Content = "Zu übersetzen Englisch";
                Label2.Content = "Übersetzung eingeben Deutsch";
            }
            else
            {
                // Von Deutsch nach Englisch
                Label1.Content = "Zu übersetzen Deutsch";
                Label2.Content = "Übersetzung eingeben Englisch";
            }
        }



    }
}

