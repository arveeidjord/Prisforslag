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
using Corinor.Presentasjon;
using Corinor.Presentasjon.Forslag;
using Corinor.Modell.CorianProdukt;

namespace Corinor.Kontroller
{
    /// <summary>
    /// Interaction logic for KundeinfoVindu.xaml
    /// </summary>
    public partial class KundeViserKontroll : UserControl
    {
        public KundeViserKontroll(ForslagPresentasjon forslag, bool somPDF)
        {
            InitializeComponent();
            //////this.tilvalgGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.datoTekst.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            this.navnTekst.Text = forslag.Navn; ;
            this.adresseTekst.Text = forslag.Adresse;
            this.postnummerTekst.Text = forslag.Postnummer;
            this.poststedTekst.Text = forslag.Poststed;
            this.epostTekst.Text = forslag.Epost;
            this.telefonnrTekst.Text = forslag.Telefonnummere;
            this.kommentarTekstblokk.Text = forslag.Kommentar;

            this.totalsumTekst.Text = forslag.Totalpris.ToString();

            if (somPDF)
                logoBilde.Margin = new Thickness(logoBilde.Margin.Left, logoBilde.Margin.Top, logoBilde.Margin.Right + 160, logoBilde.Margin.Bottom);

            init(forslag);
        }



        private void init(ForslagPresentasjon forslag)
        {
            this.merknaderTekstblokk.Text = "";
            List<Merknad> merknader = new List<Merknad>();
            oppdaterMerknader(merknader, forslag);
        }

        private void oppdaterMerknader(List<Merknad> merknader, ForslagPresentasjon forslag)
        {
            

            foreach (ProduktPresentasjon produkt in forslag.Produkter)
            {
                if (produkt.Merknader3 != null)
                {
                    foreach (Merknad merknad in produkt.Merknader3)
                    {
                        if (!merknader.Contains(merknad))
                        {
                            merknaderTekstblokk.Text += merknad.MerknadTekst + "\n";
                            merknader.Add(merknad);
                        }

                    }
                }
            }

            if (!(forslag is HeltreForslagPresentasjon))
            {
                merknaderTekstblokk.Text = merknaderTekstblokk.Text.Trim();
                return;
            }

            foreach (ProduktPresentasjon produkt in (forslag as HeltreForslagPresentasjon).Tilvalg)
            {
                if (produkt.Merknader3 != null)
                {
                    foreach (Merknad merknad in produkt.Merknader3)
                    {
                        if (!merknader.Contains(merknad))
                        {
                            merknaderTekstblokk.Text += merknad.MerknadTekst + "\n";
                            merknader.Add(merknad);
                        }

                    }
                }
            }

            merknaderTekstblokk.Text = merknaderTekstblokk.Text.Trim();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }



        //public KundeViserKontroll(HeltreForslagPresentasjon forslag) //DateTime dato, string navn, string adresse, string postnummer, string poststed, 
        //{
        //    InitializeComponent();

        //    ////////////////this.sumBenkeplaterTekst.Text = forslag.getListeSum().ToString();
        //    ////////////////this.sumTilvalgTekst.Text = forslag.getSumTilvalg().ToString();
        //    ////////////////this.tilvalgGrid.Visibility = System.Windows.Visibility.Visible;

        //    init(forslag);

        //    //merknaderTekstblokk.Text = "";

        //    //List<Merknad> merknader = new List<Merknad>();

        //    //foreach (ProduktPresentasjon produkt in forslag.Produkter)
        //    //{
        //    //    produkt.
        //    //}

        //}
    }
}
