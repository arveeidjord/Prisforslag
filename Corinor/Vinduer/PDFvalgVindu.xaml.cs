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
using System.Windows.Shapes;

namespace Corinor.Vinduer
{
    /// <summary>
    /// Interaction logic for PDFvalgVindu.xaml
    /// </summary>
    public partial class PDFvalgVindu : Window
    {
        public PDFvalgVindu(bool liggendeSomStandard)
        {
            InitializeComponent();

            this.avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);
            this.fortsettKnapp.Click += new RoutedEventHandler(fortsettKnapp_Click);
            if (liggendeSomStandard)
            {
                liggendeBoks.IsChecked = true;
                infoTekstblokk.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ståendeBoks.IsChecked = true;
                infoTekstblokk.Visibility = System.Windows.Visibility.Hidden;

            }
        }

        void fortsettKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
