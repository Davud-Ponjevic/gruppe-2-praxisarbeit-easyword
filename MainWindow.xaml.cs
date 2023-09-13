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

        public MainWindow()
        {
            InitializeComponent();

            
            data = File.ReadAllLines("C:\\Users\\Max_ipso\\Downloads\\Arbeit.csv");
            
            translator = new Translator(data);

            // Setzen Sie das Wort beim Start der Anwendung
            SetNextWord();
        }

        private void SetNextWord()
        {
            if (currentIndex >= data.Length) // Wenn am Ende der Liste, zurück zum Anfang
            {
                currentIndex = 0;
            }

            currentWord = data[currentIndex].Split(';')[translateToY ? 0 : 1];
            WordToTrans.Text = currentWord;
            currentIndex++;  // Erhöhen Sie den Index für das nächste Mal
        }

        private void Switchlangu(object sender, RoutedEventArgs e)
        {
            translateToY = !translateToY;  // Übersetzungsrichtung umschalten
            SetNextWord();
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

            SetNextWord();  // Wechseln Sie das Wort nach dem Überprüfen der Übersetzung
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

