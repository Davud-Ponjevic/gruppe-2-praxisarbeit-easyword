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
        Translator translator;
        string currentWord;  // Dies speichert das aktuelle Wort, das übersetzt werden soll

        public MainWindow()
        {
            InitializeComponent();

            // Laden Sie die CSV-Datei und erstellen Sie den Übersetzer
            string[] data = File.ReadAllLines("C:\\Users\\david\\OneDrive\\Desktop\\Arbeit.csv");
            translator = new Translator(data);
        }

        // Dies könnte ein Ereignishandler sein, der ausgelöst wird, wenn Sie die Übersetzungsrichtung ändern wollen
        private void ChangeTranslationDirection(object sender, RoutedEventArgs e)
        {
            // Beispiel: Holen Sie sich das nächste Wort aus der CSV-Datei (dies kann erweitert werden)
            currentWord = data[0].Split(';')[0];  // Nehmen Sie das erste Wort als Beispiel

            // Zeigen Sie das Wort im TextBlock an
            WordToTrans.Text = currentWord;
        }

        private void TestResu(object sender, RoutedEventArgs e)
        {
            string userTranslation = WordTransResu.Text;
            string result = translator.CheckXToY(currentWord, userTranslation);  // Für Deutsch nach Englisch
                                                                                 // Oder: 
                                                                                 // string result = translator.CheckYToX(currentWord, userTranslation);  // Für Englisch nach Deutsch

            // Zeigen Sie das Ergebnis an (z.B. in einem MessageBox oder einem anderen TextBlock)
            MessageBox.Show(result);
        }
    }

}
