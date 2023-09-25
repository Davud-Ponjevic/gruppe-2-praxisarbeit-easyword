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
using System.Windows.Threading;



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
        private bool isGermanToEnglish = true;

        // Timer wegen rückmeldung
        private DispatcherTimer feedbackTimer;



        public MainWindow()
        {
            InitializeComponent();
            SetLanguageLabels();

            // Open the file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "CSV-Datei auswählen";
            openFileDialog.Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*";



            #region Timer Konstruktor initialisieren
            //Konstruktor für timer
            feedbackTimer = new DispatcherTimer();
            feedbackTimer.Interval = TimeSpan.FromSeconds(3);
            feedbackTimer.Tick += FeedbackTimer_Tick;
            #endregion

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
            string tempCurrentWord = currentWord;
            string result;

            if (IgnoreCaseCheckBox.IsChecked == true)
            {
                // Wenn die Checkbox aktiviert ist, konvertieren Sie beide Strings in Kleinbuchstaben
                userTranslation = userTranslation.ToLower();
                tempCurrentWord = tempCurrentWord.ToLower();
            }

            if (translateToY)
            {
                result = translator.CheckXToY(tempCurrentWord, userTranslation);  // Für Deutsch nach Englisch
            }
            else
            {
                result = translator.CheckYToX(tempCurrentWord, userTranslation);  // Für Englisch nach Deutsch
            }

            // Speichern Sie das aktuelle Wort, bevor Sie es aus der Liste entfernen
            string currentWordLine = currentRoundWords[currentIndex];

            // Entfernen Sie das aktuelle Wort aus der Liste für die aktuelle Runde
            currentRoundWords.RemoveAt(currentIndex);

            // Wenn die Übersetzung falsch war, fügen Sie das Wort zur Liste für die nächste Runde hinzu
            if (result.StartsWith("Falsch"))
            {
                WordBorder.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xA0, 0xA0)); // Setzt den Hintergrund des Borders auf ein helleres Rot
                nextRoundWords.Add(currentWordLine);
            }
            else
            {
                WordBorder.Background = new SolidColorBrush(Color.FromRgb(0xA0, 0xFF, 0xA0)); // Setzt den Hintergrund des Borders auf ein helleres Grün
            }

            feedbackTimer.Start();  // Startet den Timer, um die Farbe nach 3 Sekunden zurückzusetzen
            WordTransResu.Text = "";
        }



        private void Switchlangu(object sender, RoutedEventArgs e)
        {
            translateToY = !translateToY;  // Übersetzungsrichtung umschalten
            isGermanToEnglish = !isGermanToEnglish;
            SetLanguageLabels();

            currentWord = currentRoundWords[currentIndex].Split(';')[translateToY ? 0 : 1];
            WordToTrans.Text = currentWord;
            


        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {



        }
        private void SetLanguageLabels()
        {
            if (isGermanToEnglish)
            {
                // Von Englisch nach Deutsch
                Label1.Content = "Wort übersetztn auf Englisch";
               
            }
            else
            {
                // Von Deutsch nach Englisch
                Label1.Content = "Wort übersetztn auf Deutsch";
               
            }
        }
        // eventhandler wegen timer
        private void FeedbackTimer_Tick(object sender, EventArgs e)
        {
            WordBorder.Background = new SolidColorBrush(Color.FromRgb(0xE3, 0xF2, 0xFD)); // Setzt den Hintergrund des Borders zurück
            


            feedbackTimer.Stop(); // Stoppt den Timer

            // Entfernen Sie das aktuelle Wort aus der Liste für die aktuelle Runde
            currentRoundWords.RemoveAt(currentIndex);
            

            SetNextWord();
            
        }




    }
}

