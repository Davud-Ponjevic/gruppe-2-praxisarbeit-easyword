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
        private List<string> wordsToAsk;

        public MainWindow()
        {
            InitializeComponent();

            data = File.ReadAllLines("C:\\Users\\david\\OneDrive\\Desktop\\Arbeit.csv");
            translator = new Translator(data);

            // Initialisieren Sie die Liste mit allen Wörtern aus der CSV-Datei
            wordsToAsk = new List<string>(data);

            // Setzen Sie das erste Wort
            SetNextWord();
        }

        private void SetNextWord()
        {
            // Wenn alle Wörter korrekt übersetzt wurden
            if (wordsToAsk.Count == 0)
            {
                MessageBox.Show("Alle Wörter wurden korrekt übersetzt!");
                // Optional: Schließen Sie die Anwendung oder laden Sie eine neue Liste
                this.Close();
                return;
            }

            // Wählen Sie zufällig ein Wort aus der Liste
            Random random = new Random();
            currentIndex = random.Next(0, wordsToAsk.Count);

            currentWord = wordsToAsk[currentIndex].Split(';')[translateToY ? 0 : 1];
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

            // Zeigen Sie das Ergebnis an
            MessageBox.Show(result);
            WordTransResu.Text = "";

            // Wenn die Übersetzung korrekt war, entfernen Sie das Wort aus der Liste
            if (!result.StartsWith("Falsch"))
            {
                wordsToAsk.RemoveAt(currentIndex);
            }

            SetNextWord();  // Wechseln Sie das Wort nach dem Überprüfen der Übersetzung
        }
        private void Switchlangu(object sender, RoutedEventArgs e)
        {
            translateToY = !translateToY;  // Übersetzungsrichtung umschalten
            SetNextWord();
        }


    }
}

