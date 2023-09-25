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
using Newtonsoft.Json;




namespace Transalto
{
    public partial class MainWindow : Window
    {
        #region Listen und OBJ anlegen
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

        // für wort statistik
        private Dictionary<string, int> wordMistakes = new Dictionary<string, int>();
        #endregion


        #region Speichern des files
        // Liste der Dateien
        private string jsonFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "files.json");
        private List<string> files = new List<string>();



        #endregion

        
        #region MainCode

        public MainWindow()
        {
            InitializeComponent();
            SetLanguageLabels();
            LoadFilesFromJson();

            if(files.Count > 0 ) 
            {
                // Wenn bereits Dateien vorhanden sind, laden Sie die Daten aus der ersten Datei in der Liste.
                foreach (string file in files)
                {

                    if (!File.Exists(file))
                    {
                        // Entfernen Sie die ausgewählte Datei aus der Liste
                        files.Remove(file);

                        // Speichern Sie die aktualisierte Dateiliste in der JSON-Datei
                        
                    }
                   
                };
                SaveFilesToJson();

            }

            if (files.Count == 0)
            {
                MessageBoxResult result = MessageBox.Show("Bitte fügen Sie eine Datei hinzu.", "Datei hinzufügen", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = "CSV-Datei auswählen";
                    openFileDialog.Filter = "CSV-Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        
                        files.Add(openFileDialog.FileName);
                        SaveFilesToJson();
                        data = File.ReadAllLines(openFileDialog.FileName);
                    }
                    else
                    {
                        // Wenn der Benutzer den Dialog abbricht, ohne eine Datei auszuwählen, schließen Sie das Programm.
                        this.Close();
                        return;
                    }
                }
                else
                {
                    // Wenn der Benutzer den Dialog abbricht, ohne eine Datei auszuwählen, schließen Sie das Programm.
                    this.Close();
                    return;
                }
            }
            else
            {
                data = File.ReadAllLines(files[0]);
            }
            

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

            #region Timer Konstruktor initialisieren
            //Konstruktor für timer
            feedbackTimer = new DispatcherTimer();
            feedbackTimer.Interval = TimeSpan.FromSeconds(1.5);
            feedbackTimer.Tick += FeedbackTimer_Tick;
            #endregion
        }

        #endregion

        #region Functionen zum spreichen und holen der json files
        void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            // Dateiliste aktualisieren
            FileList.ItemsSource = files;
            FilePopup.IsOpen = !FilePopup.IsOpen;
        }

         void AddNewFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string FullPathOnly = System.IO.Path.GetFullPath(openFileDialog.FileName);
                files.Add(FullPathOnly);
                SaveFilesToJson();

                // Daten aus der neuen Datei laden
                data = File.ReadAllLines(openFileDialog.FileName);

                // Überprüfen Sie das Format jeder Zeile
                foreach (var line in data)
                {
                    var entries = line.Split(';');
                    if (entries.Length != 2 || string.IsNullOrWhiteSpace(entries[0]) || string.IsNullOrWhiteSpace(entries[1]))
                    {
                        MessageBox.Show($"Fehler im Format der Zeile: {line}. Jede Zeile sollte ein deutsches Wort und eine englische Übersetzung haben, getrennt durch ein Semikolon.");
                        return;  // Beenden Sie die Verarbeitung, wenn ein Fehler gefunden wird
                    }
                }

                translator = new Translator(data);

                // Initialisieren Sie die Listen für die aktuelle Runde und die nächste Runde
                currentRoundWords = new List<string>(data);
                nextRoundWords = new List<string>();

                SetNextWord();

                LoadFilesFromJson();  // Laden Sie die aktualisierte Dateiliste
                FileList.ItemsSource = null;  // Setzen Sie die Datenquelle zurück
                FileList.ItemsSource = files;  // Weisen Sie die aktualisierte Dateiliste erneut zu

            }
        }

         void SaveFilesToJson()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(files);
            File.WriteAllText(jsonFilePath, json); // Verwenden Sie den festgelegten Pfad
        }

         void LoadFilesFromJson()
        {
            if (File.Exists(jsonFilePath)) // Verwenden Sie den festgelegten Pfad
            {
                string json = File.ReadAllText(jsonFilePath); // Verwenden Sie den festgelegten Pfad
                files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);


            }
        }
        #endregion

        #region Next Word
         void SetNextWord()
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
        #endregion

        #region testen der Eingabe testResu
         void TestResu(object sender, RoutedEventArgs e)
        {
            string userTranslation = WordTransResu.Text;
            string tempCurrentWord = currentWord;
            string result;

            string currentWordLine = currentRoundWords[currentIndex];


            if (IgnoreCaseCheckBox.IsChecked == true) 
            {
                userTranslation = userTranslation.ToLower();
                tempCurrentWord = tempCurrentWord.ToLower();
            }

            if (translateToY)
            {
                result = translator.CheckXToY(tempCurrentWord, userTranslation);
            }
            else
            {
                result = translator.CheckYToX(tempCurrentWord, userTranslation);
            }

            // ... (Rest des Codes bleibt unverändert)

            // Wenn die Übersetzung falsch war, fügen Sie das Wort zur Liste für die nächste Runde hinzu und aktualisieren Sie die Statistik
            if (result.StartsWith("Falsch"))
            {
                WordBorder.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xA0, 0xA0));
                nextRoundWords.Add(currentWordLine);

                // Update wordMistakes dictionary
                if (wordMistakes.ContainsKey(currentWord))
                {
                    wordMistakes[currentWord]++;
                }
                else
                {
                    wordMistakes[currentWord] = 1;
                }
                SaveMistakesToFile(); // Speichern Sie die Fehler jedes Mal, wenn sie auftreten
            }
            else
            {
                WordBorder.Background = new SolidColorBrush(Color.FromRgb(0xA0, 0xFF, 0xA0));
            }

            feedbackTimer.Start();  // Startet den Timer, um die Farbe nach 3 Sekunden zurückzusetzen
            WordTransResu.Text = "";
        }

        #endregion

        #region Sprache wächseln
         void Switchlangu(object sender, RoutedEventArgs e)
        {
            translateToY = !translateToY;  // Übersetzungsrichtung umschalten
            isGermanToEnglish = !isGermanToEnglish;
            SetLanguageLabels();

            currentWord = currentRoundWords[currentIndex].Split(';')[translateToY ? 0 : 1];
            WordToTrans.Text = currentWord;
            


        }
        #endregion

        #region Textbox und lables
        void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {



        }
        void SetLanguageLabels()
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
        #endregion

        #region bastand zwischen wort wächseln (Timer)
        // eventhandler wegen timer
        void FeedbackTimer_Tick(object sender, EventArgs e)
        {
            WordBorder.Background = new SolidColorBrush(Color.FromRgb(0xE3, 0xF2, 0xFD)); // Setzt den Hintergrund des Borders zurück
            


            feedbackTimer.Stop(); // Stoppt den Timer

            // Entfernen Sie das aktuelle Wort aus der Liste für die aktuelle Runde
            currentRoundWords.RemoveAt(currentIndex);
            

            SetNextWord();
            
        }
        #endregion

        #region Stats von Fehlern

        void SaveMistakesToFile()
        {
            File.WriteAllLines("mistakes.txt", wordMistakes.Select(kvp => $"{kvp.Key};{kvp.Value}"));
        }

        
        void LoadMistakesFromFile()
        {
            if (File.Exists("mistakes.txt"))
            {
                foreach (var line in File.ReadAllLines("mistakes.txt"))
                {
                    var parts = line.Split(';');
                    wordMistakes[parts[0]] = int.Parse(parts[1]);
                }
            }
        }

        void ShowStats(object sender, RoutedEventArgs e)
        {
            var stats = string.Join("\n", wordMistakes.Select(kvp => $"{kvp.Key}: {kvp.Value} mal"));
            MessageBox.Show(stats);
        }

        void ResetStats(object sender, RoutedEventArgs e)
        {
            wordMistakes.Clear();
            SaveMistakesToFile();
        }


        void OnStatsButtonClick(object sender, RoutedEventArgs e)
        {
            StatsPopup.IsOpen = !StatsPopup.IsOpen;
        }

        #endregion

        #region Neue wörter akzeptieren und nutzuen
        private void OnFilePopupOkClick(object sender, RoutedEventArgs e)
        {
            // Überprüfen Sie, ob eine Datei aus der Liste ausgewählt wurde
            if (FileList.SelectedItem != null)
            {
                string selectedFile = FileList.SelectedItem.ToString();

                // Laden Sie die Daten aus der ausgewählten Datei
                data = File.ReadAllLines(selectedFile);

                // Überprüfen Sie das Format jeder Zeile
                foreach (var line in data)
                {
                    var entries = line.Split(';');
                    if (entries.Length != 2 || string.IsNullOrWhiteSpace(entries[0]) || string.IsNullOrWhiteSpace(entries[1]))
                    {
                        MessageBox.Show($"Fehler im Format der Zeile: {line}. Jede Zeile sollte ein deutsches Wort und eine englische Übersetzung haben, getrennt durch ein Semikolon.");
                        return;  // Beenden Sie die Verarbeitung, wenn ein Fehler gefunden wird
                    }
                }

                translator = new Translator(data);

                // Initialisieren Sie die Listen für die aktuelle Runde und die nächste Runde
                currentRoundWords = new List<string>(data);
                nextRoundWords = new List<string>();

                SetNextWord();

                // Schließen Sie das Popup
                FilePopup.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Datei aus der Liste aus.");
            }
        }
        #endregion

        #region File delete
        private void OnFileDeleteClick(object sender, RoutedEventArgs e)
        {
            // Überprüfen Sie, ob eine Datei aus der Liste ausgewählt wurde
            if (FileList.SelectedItem != null)
            {
                string selectedFile = FileList.SelectedItem.ToString();

                // Entfernen Sie die ausgewählte Datei aus der Liste
                files.Remove(selectedFile);

                // Aktualisieren Sie die Dateiliste im Popup
                FileList.ItemsSource = null;
                FileList.ItemsSource = files;

                // Speichern Sie die aktualisierte Dateiliste in der JSON-Datei
                SaveFilesToJson();
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Datei aus der Liste aus, die Sie löschen möchten.");
            }
        }
        #endregion

    }
}

