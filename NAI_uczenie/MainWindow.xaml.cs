using NAI_uczenie;
using NAI_uczenie.Tools;
using NeuronNetwork_CharLearning.Models;
using System;
using System.Collections.ObjectModel;
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
        public ObservableCollection<InputData> InputsDatas { get; set; } = new ObservableCollection<InputData>();

        private NeuronNetwork NeuronNetwork { get; set; }
        private double[] X_GUI_Vector { get; set; }

        private readonly DoubleAnimation animation = new DoubleAnimation();
        private MyChart chart;

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

            NeuronNetwork = new NeuronNetwork(InputsDatas, new DataReader(InputsDatas).ReadData());
            var errors = NeuronNetwork.Teach();

            Era_TextBox.Text = $"LAST\nERA:\n{errors.Length - 1}";
            LastError_TextBox.Text = $"LAST\nERROR:\n{Math.Round(errors[errors.Length - 1], 2)}";

            chart.InputData(errors);
        }

        private void Check_Btn_Click(object sender, RoutedEventArgs e)
        {
            Result_TextBox.Text = $"{NeuronNetwork.Test(X_GUI_Vector)}";
            Result_TextBox.BeginAnimation(TextBox.FontSizeProperty, animation);
        }

        private void GenerateButtons(int coliumnSize, int rowsSize)
        {
            for (int i = 0; i < rowsSize; i++)
            {
                for (int j = 0; j < coliumnSize; j++)
                {
                    var btn = new Button();
                    btn.Click += UserCharsDrawingBtn_Click;
                    btn.Name = $"b{j + i * coliumnSize}";
                    btn.Background = Brushes.White;
                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, i);
                    userCharsDrawingGrid.Children.Add(btn);
                }
            }
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
            learn_Btn.Focus();
            chart = new MyChart(Chart);
            PrepareAnimation();
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
            else
            {
                X_GUI_Vector[position] = 0;
                btn.Background = Brushes.White;
            }
            check_Btn.Focus();
        }
    }
}