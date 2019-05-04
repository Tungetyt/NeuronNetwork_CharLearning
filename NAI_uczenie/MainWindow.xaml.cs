using NeuronNetwork_CharLearning.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.IO;
using NeuronNetwork_CharLearning.Properties;

namespace NeuronNetwork_CharLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<InputData> InputsDatas { get; set; } = new ObservableCollection<InputData>();
        double[] X_GUI_Vector { get; set; }
        NeuronNetwork NeuronNetwork { get; set; }

        public MainWindow()
        {
            WindowState = WindowState.Maximized;
            InitializeComponent();

            Chars_ListBox.ItemsSource = InputsDatas;
            Result_TextBox.IsReadOnly = true;
            Era_TextBox.IsReadOnly = true;
            LastError_TextBox.IsReadOnly = true;
            learn_Btn.IsEnabled = true;
            learn_Btn.Focus();

            int coliumnSize = 4;
            int rowsSize = 6;

            X_GUI_Vector = new double[coliumnSize * rowsSize];

            //Generowanie dynamiczne przyciskow do wprowadzania znaku
            GenerateButtons(coliumnSize, rowsSize);
        }

        private void GenerateButtons(int coliumnSize, int rowsSize)
        {
            for (int i = 0, counter = 0; i < rowsSize; i++)
            {
                for (var j = 0; j < coliumnSize; j++)
                {
                    var btn = new Button();
                    btn.Click += UserCharsDrawingBtn_Click;
                    btn.Name = "b" + counter;
                    btn.Background = Brushes.White;
                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, i);
                    userCharsDrawingGrid.Children.Add(btn);

                    counter++;
                }
            }
        }

        //Oprogramowanie przyciskow do wprowadzania znaku
        void UserCharsDrawingBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var position = Int32.Parse(Regex.Replace(btn.Name, "b", ""));
            if (X_GUI_Vector[position] == 0)
            {
                X_GUI_Vector[position] = 1;
                btn.Background = Brushes.Blue;
            }
            else if (X_GUI_Vector[position] == 1)
            {
                X_GUI_Vector[position] = 0;
                btn.Background = Brushes.White;
            }
            check_Btn.Focus();
        }

        public void Learn_Btn_Click(object sender, RoutedEventArgs e)
        {
            Result_TextBox.Text = "";
            check_Btn.IsEnabled = true;

            ReadDataToINPUT_DATAS();

            NeuronNetwork = new NeuronNetwork(InputsDatas);
            var errors = NeuronNetwork.Teach();
            Era_TextBox.Text = $"LAST\nERA:\n{errors.Length - 1}";
            LastError_TextBox.Text = $"LAST\nERROR:\n{Math.Round(errors[errors.Length - 1], 2)}";

            InputChartData(errors);
        }

        private void InputChartData(double[] errors)
        {
            ChartValues<ObservablePoint> points = new ChartValues<ObservablePoint>();
            for (int i = 1; i < errors.Count(); i++)
            {
                points.Add(new ObservablePoint
                {
                    X = i,
                    Y = errors[i]
                });
            }
            var SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = points
                }
            };
            Chart.Series = SeriesCollection;
        }

        private void ReadDataToINPUT_DATAS()
        {
            InputsDatas.Clear();
            var path = $@"{AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug\", "")}Data\data.txt";

            using (StreamReader file = new StreamReader(path))
            {
                string ln;
                Dictionary<char, int[]> biggusDicus = new Dictionary<char, int[]>();
                int counter = 0;

                while ((ln = file.ReadLine()) != null)
                {
                    char label = ln[ln.Length - 1];
                    double[] xVector = Regex.Replace(ln, @",.", "").Select(c => (double)(c - '0')).ToArray();

                    if (biggusDicus.ContainsKey(label))
                    {
                        InputsDatas.Add(new InputData(xVector, label, biggusDicus[label]));
                    }
                    else
                    {
                        int[] dArr = new int[NeuronNetwork.maxOutputNeurons];
                        dArr[counter] = 1;
                        counter++;
                        biggusDicus.Add(label, dArr);
                        InputsDatas.Add(new InputData(xVector, label, dArr));
                    }
                }
            }
        }

        void Check_Btn_Click(object sender, RoutedEventArgs e)
        {
            Result_TextBox.Text = $"FOUND: {NeuronNetwork.Test(X_GUI_Vector)}";
        }
    }
}