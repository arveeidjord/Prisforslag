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
using Corinor.Kontrollbehandling;
using Corinor.Presentasjon;
using Corinor.Vinduer;
using Corinor.DataAksess;
using Corinor.Presentasjon.Forslag;
using System.IO;
using System.Diagnostics;

namespace Corinor.Kontroller.Forslag
{
    /// <summary>
    /// Interaction logic for PrismodellKontroll.xaml
    /// </summary>
    public partial class HeltreForslagKontroll : UserControl
    {
        private static int antallForslag = 0;
        HeltreForslagService heltreForslagService;

        public string ForslagKontrollTittel { get { return string.Format("Prisforslag {0} (heltre)", ++antallForslag);}}

        public HeltreForslagKontroll(DataAksess2 db)
        {
            InitializeComponent();

            heltreForslagService = new HeltreForslagService(db);

            //#Hardkoding av standard tilvalg
            foreach (Modell.Tilvalg.TilvalgGruppe item in db.Produktbeholder.Tilvalgliste)
            {
                if (item.GruppeTittel.ToLower().Contains("profilfresing") && item.tilvalgListe.Count > 0)
                {
                    TilvalgEnkelPrisTusendel k = new TilvalgEnkelPrisTusendel(item.tilvalgListe[0].TilvalgTittel, item.tilvalgListe[0].Pris);
                    k.AntallEnheter = 0;
                    heltreForslagService.ForslagSomVises.AddTilvalg(k);
                    break;
                }
          
            }

            //foreach (Modell.Tilvalg.TilvalgGruppe item in db.Produktbeholder.Tilvalgliste)
            //{
            //    if (item.GruppeTittel.ToLower().Contains("vedlikeholdspakke") && item.tilvalgListe.Count > 0)
            //    {
            //        KontrollProduktPresentasjon k = new TilvalgEnkelPris(item.tilvalgListe[0].TilvalgTittel, item.tilvalgListe[0].Pris);
            //        heltreForslagService.ForslagSomVises.AddTilvalg(k);
            //        break;
            //    }
            //}
            //###

            nyHeltreKnapp.Click += nyHeltreKnapp_Click;
            kummerKnapp.Click += kummerKnapp_Click;
            annetTilvalgKnapp.Click += annetTilvalgKnapp_Click;
            kundeKnapp.Click += kundeKnapp_Click;
            print.Click += print_Click;
            sendTilEpostKnapp.Click += sendTilEpostKnapp_Click;
            lagreSomPDF.Click += lagreSomPDF_Click;

            forslagListBoks.DataContext = null;
            forslagListBoks.DataContext = heltreForslagService.ForslagSomVises;

            tilvalgListView.DataContext = null;
            tilvalgListView.DataContext = heltreForslagService.ForslagSomVises;

            totalprisLabel.DataContext = null;
            totalprisLabel.DataContext = heltreForslagService.ForslagSomVises;

            totalprisIScrollLabel.DataContext = null;
            totalprisIScrollLabel.DataContext = heltreForslagService.ForslagSomVises;
        }

 

        void kummerKnapp_Click(object sender, RoutedEventArgs e)
        {
            heltreForslagService.leggTilKummer(this);
        }

       

        void kundeKnapp_Click(object sender, RoutedEventArgs e)
        {
            heltreForslagService.endreKundeInfo(this);
        }

        //Remove
        private void heltreplateItemButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem clickedItem = Corinor.Hjelpeklasser.VisualTree.getParentKontroll<ListViewItem>(sender as DependencyObject);
            HeltreplatePresentasjon produkt =  forslagListBoks.ItemContainerGenerator.ItemFromContainer(clickedItem) as HeltreplatePresentasjon;

            heltreForslagService.ForslagSomVises.Remove(produkt);
        }

        private void tilvalgItemButton_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem clickedItem = Corinor.Hjelpeklasser.VisualTree.getParentKontroll<ListViewItem>(sender as DependencyObject);
            KontrollProduktPresentasjon produkt = tilvalgListView.ItemContainerGenerator.ItemFromContainer(clickedItem) as KontrollProduktPresentasjon;

            heltreForslagService.ForslagSomVises.RemoveTilvalg(produkt);
        }


        void lagreSomPDF_Click(object sender, RoutedEventArgs e)
        {
            if (!klarTilPrint()) return;

            double heltreColumnLengde = 0;
            foreach (GridViewColumn c in gridView.Columns)
                heltreColumnLengde += c.ActualWidth;

            bool liggendeAnbefalt = (produktColumn.ActualWidth > 435.0 || heltreColumnLengde > 655);

            PDFvalgVindu pdfValgVindu = new PDFvalgVindu(liggendeAnbefalt);
            pdfValgVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(this);
            pdfValgVindu.ShowDialog();

            if (pdfValgVindu.DialogResult != true) return;
            bool liggende = (bool)pdfValgVindu.liggendeBoks.IsChecked;


            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.CheckPathExists = true;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.ShowHelp = false;
            dlg.OverwritePrompt = true;
            dlg.Filter = "PDF-fil|*.pdf";
            dlg.Title = "Velg hvor PDF-filen skal lagres";

            System.Windows.Forms.DialogResult res = dlg.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                KundeViserKontroll kundeKontroll = new KundeViserKontroll(heltreForslagService.ForslagSomVises, true);
                string pdfUrl = Printer.PrintSom(kundeKontroll, scrollKontroll, true, dlg.FileName, heltreKnappeGridColumn, tilvalgKnappeGridColumn, liggende);

                try
                {
                    if (File.Exists(pdfUrl))
                        Process.Start(pdfUrl);
                }
                catch (Exception)
                {
                    MessageBox.Show("Klarte ikke å vise prisforslaget som ble lagret til:\n\n" + pdfUrl);
                }
            }
        }


        void sendTilEpostKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (!klarTilPrint()) return;

            double heltreColumnLengde = 0;
            foreach (GridViewColumn c in gridView.Columns)
                heltreColumnLengde += c.ActualWidth;

            bool liggendeAnbefalt = (produktColumn.ActualWidth > 435.0 || heltreColumnLengde > 655);

            PDFvalgVindu pdfValgVindu = new PDFvalgVindu(liggendeAnbefalt);
            pdfValgVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(this);
            pdfValgVindu.ShowDialog();

            if (pdfValgVindu.DialogResult != true) return;
            bool liggende = (bool) pdfValgVindu.liggendeBoks.IsChecked;
            
            KundeViserKontroll kundeKontroll = new KundeViserKontroll(heltreForslagService.ForslagSomVises, true);
            string pdfUrl = Printer.PrintSom(kundeKontroll, scrollKontroll, true, "", heltreKnappeGridColumn, tilvalgKnappeGridColumn, liggende);

            if (File.Exists(pdfUrl))
                Hjelpeklasser.EpostSender.SendEpost(pdfUrl, "Prisforslag", heltreForslagService.ForslagSomVises.Epost);

        }

        void print_Click(object sender, RoutedEventArgs e)
        {
            if (!klarTilPrint()) return;

            double heltreColumnLengde = 0;
            foreach (GridViewColumn c in gridView.Columns)
                heltreColumnLengde += c.ActualWidth;

            if (produktColumn.ActualWidth > 435.0 || heltreColumnLengde > 655)
                MessageBox.Show("For dette prisforslaget er det anbefalt å bruke liggende format.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Information);

            KundeViserKontroll kundeKontroll = new KundeViserKontroll(heltreForslagService.ForslagSomVises, false);
            Printer.PrintSom(kundeKontroll, scrollKontroll, false, "", heltreKnappeGridColumn, tilvalgKnappeGridColumn, false);
        }


        bool klarTilPrint()
        {
            if (heltreForslagService == null || heltreForslagService.ForslagSomVises == null)
                return false;

            if (string.IsNullOrEmpty(heltreForslagService.ForslagSomVises.Navn))
            {
                MessageBox.Show("Du må legge til kundeinformasjon før du kan gå videre.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            produktColumn.Width = double.NaN;
            
            forslagListBoks.UpdateLayout();
            return true;
        }


        void annetTilvalgKnapp_Click(object sender, RoutedEventArgs e)
        {
            heltreForslagService.leggTilAnnetTilvalg(this);
        }

        void nyHeltreKnapp_Click(object sender, RoutedEventArgs e)
        {
            heltreForslagService.leggTilNyHeltreplate(this);
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
