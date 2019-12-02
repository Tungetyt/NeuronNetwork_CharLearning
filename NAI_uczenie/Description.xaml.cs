using System;
using System.Windows;

namespace NAI_uczenie
{
    /// <summary>
    /// Interaction logic for Description.xaml
    /// </summary>
    public partial class Description : Window
    {
        public Description()
        {
            InitializeComponent();
            description_TextBox.Text = System.IO.File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug\", "")}README.txt");
        }
    }
}