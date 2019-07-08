using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NeuronNetwork_CharLearning.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NeuronNetwork_CharLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<InputData> InputsDatas { get; set; } = new ObservableCollection<InputData>();

        private NeuronNetwork NeuronNetwork { get; set; }
        private double[] X_GUI_Vector { get; set; }

        private readonly DoubleAnimation animation = new DoubleAnimation();

        private int NumOfDistinctLabels { get; set; }

        public MainWindow()
        {
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
            PrepareControls();

            int coliumnSize = 4;
            int rowsSize = 6;

            X_GUI_Vector = new double[coliumnSize * rowsSize];

            GenerateButtons(coliumnSize, rowsSize);
        }

        private void Learn_Btn_Click(object sender, RoutedEventArgs e)
        {
            Result_TextBox.Text = "";
            check_Btn.IsEnabled = true;

            ReadData();

            NeuronNetwork = new NeuronNetwork(InputsDatas, NumOfDistinctLabels);
            var errors = NeuronNetwork.Teach();

            Era_TextBox.Text = $"LAST\nERA:\n{errors.Length - 1}";
            LastError_TextBox.Text = $"LAST\nERROR:\n{Math.Round(errors[errors.Length - 1], 2)}";

            InputChartData(errors);
        }

        private void Check_Btn_Click(object sender, RoutedEventArgs e)
        {
            Result_TextBox.Text = $"{NeuronNetwork.Test(X_GUI_Vector)}";
            Result_TextBox.BeginAnimation(TextBox.FontSizeProperty, animation);
        }

        private void GenerateButtons(int coliumnSize, int rowsSize)
        {
            for (int i = 0, counter = 0; i < rowsSize; i++)
            {
                for (int j = 0; j < coliumnSize; j++)
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

        private void InputChartData(double[] errors)
        {
            Chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = GetPoints(errors)
                }
            };
        }

        private ChartValues<ObservablePoint> GetPoints(double[] errors)
        {
            var points = new ChartValues<ObservablePoint>();
            for (int i = 1; i < errors.Count(); i++)
            {
                points.Add(new ObservablePoint
                {
                    X = i,
                    Y = errors[i]
                });
            }

            return points;
        }

        private void PrepareAnimation()
        {
            animation.From = 35;
            animation.To = 45;
            animation.AutoReverse = true;
            animation.RepeatBehavior = new RepeatBehavior(2);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.75));
        }

        private void PrepareControls()
        {
            Chars_ListBox.ItemsSource = InputsDatas;
            Result_TextBox.IsReadOnly = true;
            Era_TextBox.IsReadOnly = true;
            LastError_TextBox.IsReadOnly = true;
            learn_Btn.IsEnabled = true;
            learn_Btn.Focus();
            PrepareChart();
            PrepareAnimation();
        }

        private void PrepareChart()
        {
            Chart.AxisY.Clear();
            Chart.AxisY.Add(new Axis
            {
                MinValue = 0
            });
        }

        private void ReadData()
        {
            InputsDatas.Clear();
            var path = $@"{AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug\", "")}Data\data.txt";

            using StreamReader file = new StreamReader(path);
            PopulateInputsDatas(file);
        }

        private void PopulateInputsDatas(StreamReader file)
        {
            string ln;
            var biggusDicus = new Dictionary<char, int[]>();
            NumOfDistinctLabels = 0;

            while ((ln = file.ReadLine()) != null)
            {
                AddDataToInputsDatas(biggusDicus, ln);
            }
        }

        private void AddDataToInputsDatas(Dictionary<char, int[]> biggusDicus, string ln)
        {
            char label = ln[ln.Length - 1];
            double[] xVector = Regex.Replace(ln, @",.", "").Select(c => (double)(c - '0')).ToArray();

            if (biggusDicus.ContainsKey(label))
            {
                InputsDatas.Add(new InputData(xVector, label, biggusDicus[label]));
            }
            else
            {
                int[] dArr = new int[NeuronNetwork.MaxOutputNeurons];
                dArr[NumOfDistinctLabels] = 1;
                NumOfDistinctLabels++;
                biggusDicus.Add(label, dArr);
                InputsDatas.Add(new InputData(xVector, label, dArr));
            }
        }

        private void UserCharsDrawingBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            int position = Int32.Parse(Regex.Replace(btn.Name, "b", ""));
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
    }
}