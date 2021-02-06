using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Labirintus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cbBeolvas.IsChecked = true;
        }
        

        private void BtnIndit_Click(object sender, RoutedEventArgs e)
        {
            if (cbBeolvas.IsChecked==true)
            {
                string fajlnev = tbFajlnev.Text;
                Jatekter jatekter = new Jatekter(fajlnev);
                jatekter.Show();
                this.Close();
            }
            else
            {
                Jatekter jatekter = new Jatekter();
                jatekter.Show();
                this.Close();
            }
        }

        private void BtnKilepes_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult eredmeny = MessageBox.Show("Biztosan ki akar lépni", "Kilépés a programból", MessageBoxButton.YesNo);
            if (eredmeny==MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void CbBeolvas_Checked(object sender, RoutedEventArgs e)
        {
            //tbFajlnev.Text = "";
            tbFajlnev.IsEnabled = true;
        }

        private void CbGeneral_Checked(object sender, RoutedEventArgs e)
        {
            //tbFajlnev.Text = "";
            tbFajlnev.IsEnabled = false;
        }
    }
}
