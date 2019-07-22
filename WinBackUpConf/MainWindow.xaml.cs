using Microsoft.Win32;
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

namespace WinBackUpConf
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Configuration CurrentConfiguration = new Configuration();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnBackUp_Click(object sender, RoutedEventArgs e)
        {
            CurrentConfiguration.Save(txtFile.Text);
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "WinBackUpConf file (*.wbc)|*.wbc";
            f.ShowDialog();
            txtFile.Text = f.FileName;
        }
    }
}
