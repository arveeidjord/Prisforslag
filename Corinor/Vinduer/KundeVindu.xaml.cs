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
using Corinor.Presentasjon.Forslag;
using System.Text.RegularExpressions;

namespace Corinor.Vinduer
{
    /// <summary>
    /// Interaction logic for KundeVindu.xaml
    /// </summary>
    public partial class KundeVindu : Window
    {

        public KundeVindu(ForslagPresentasjon forslag)
        {
            InitializeComponent();
            avbrytKnapp.Click += avbrytKnapp_Click;
            okKnapp.Click += okKnapp_Click;

            kommentarTekstboks.TextChanged += kommentarTekstboks_TextChanged;
            kommentarTekstboks.PreviewKeyDown += new KeyEventHandler(kommentarTekstboks_PreviewKeyDown);
            //kommentarTekstboks.PreviewTextInput += new TextCompositionEventHandler(kommentarTekstboks_PreviewTextInput);

            navnTekstboks.Text = forslag.Navn;
            adresseTekstboks.Text = forslag.Adresse;
            postnummerTekstboks.Text = forslag.Postnummer;
            poststedTekstboks.Text = forslag.Poststed;

            epostTekstboks.Text = forslag.Epost;
            telefonnummereTekstboks.Text = forslag.Telefonnummere;
            kommentarTekstboks.Text = forslag.Kommentar;

            
        }

        private static int MAX_LINES = 4;

        //Bare tillat 4 linjer
        void kommentarTekstboks_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                if (txt.LineCount >= MAX_LINES)
                    e.Handled = true;
            }
        }

        //void kommentarTekstboks_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    //TextBox txt = sender as TextBox;

        //    //string bokstav = e.Text;
        //    //bool res = Regex.IsMatch(bokstav, ".", RegexOptions.IgnoreCase);
        //    //if (res && bokstav.Length == 1)
        //    //{
        //    //    int lengde = txt.GetLineLength(txt.GetLineIndexFromCharacterIndex(txt.CaretIndex));
        //    //    if (lengde > 6)
        //    //        e.Handled = true;
        //    //}
        //}


        //Når vi limer inn text viser vi bare de tre første
        void kommentarTekstboks_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.TextChanged -= kommentarTekstboks_TextChanged;

            if (txt.LineCount > MAX_LINES)
            {
                int index = txt.CaretIndex;
                string s = "";
                for (int i = 0; i < MAX_LINES; i++)
                    s += txt.GetLineText(i);
                txt.Text = s.TrimEnd();
                txt.CaretIndex = index;
            }
            txt.TextChanged += kommentarTekstboks_TextChanged;

        }

        void okKnapp_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

  
    }
}

