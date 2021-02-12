using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Media.TextFormatting;

namespace Program2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        Random rand = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input.Trim(), @"\s+");       // split into array of words
            }
        }

        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + order);
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            if (orderComboBox.SelectedIndex == 0)
                MessageBox.Show("Please choose a order greater than 0 to babble.");
            else
            {
                textBlock1.Text = "";
                babble(words, orderComboBox.SelectedIndex);
            }
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }

        private Dictionary<string, ArrayList> makeHashtable(string[] arr, int order)
        {
            Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();
            for (int i = 0; i < words.Length; i++)
            {
                // make keys for hashtable depending on the order
                string word = "";
                if (order == 1)
                    word = words[i];
                else
                {
                    if (i <= (arr.Length - order))
                    {
                        for (int a = 0; a < order - 1; a++)
                            word += words[i + a] + "_";
                        word += words[i + order - 1];
                    }
                }
                // add values to keys in hashtable
                if (word != "")
                {
                    if (!hashTable.ContainsKey(word))
                        hashTable.Add(word, new ArrayList());
                    if (i < (arr.Length - order))
                        hashTable[word].Add(words[i + order]);
                }
            }
            return hashTable;
        }

        private void babble(string[] wordList, int order)
        {
            Dictionary<string, ArrayList> hashTable = makeHashtable(wordList, order);
            // starts babble with the first word(s) in the input file
            string firstWord = "";
            string next;
            for (int i = 0; i < order; i++)
                firstWord += wordList[i] + " ";
            textBlock1.Text += firstWord;
            // generate first key with the first word(s)
            string key = String.Join("_", firstWord.Trim().Split(' '));
            //babble
            for (int i = 0; i < Math.Min(wordCount, wordList.Length); i++)
            {
                next = generateWord(key, hashTable);
                if (next != "")
                {
                    textBlock1.Text += next + " ";
                    key = (order == 1) ? next : key.Substring(key.IndexOf('_') + 1) + "_" + next;
                }
                else
                    key = String.Join("_", firstWord.Trim().Split(' '));
            }
        }

        private string generateWord(string key, Dictionary<string, ArrayList> hashTable)
        {
            // generate a random word in the array list corresponding to the key in hashtable   
            string nextWord = "";
            ArrayList arr = hashTable[key];
            if (arr.Count != 0)
                nextWord = arr[rand.Next(arr.Count)].ToString();
            return nextWord;
        }
    }
}
