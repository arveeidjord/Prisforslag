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
using Corinor.Presentasjon.Forslag;
using Corinor.Vinduer;
using Corinor.DataAksess;
using Corinor.Presentasjon;
using System.IO;
using System.Diagnostics;

namespace Corinor.Kontroller.Forslag
{
    /// <summary>
    /// Interaction logic for CorianForslagKontroll.xaml
    /// </summary>
    public partial class CorianForslagKontroll : UserControl
    {
        private static int antallForslag = 0;
        public string ForslagKontrollTittel { get { return string.Format("Prisforslag {0} (Corian)", ++antallForslag); } }


        CorianForslagService corianForslagService;

        public CorianForslagKontroll(DataAksess2 db)
        {
            InitializeComponent();

            corianForslagService = new CorianForslagService(db);

            nyCorianplateKnapp.Click += new RoutedEventHandler(nyCorianplateKnapp_Click);
            annetTilvalgKnapp.Click += new RoutedEventHandler(annetTilvalgKnapp_Click);
            print.Click += new RoutedEventHandler(print_Click);
            sendTilEpostKnapp.Click += new RoutedEventHandler(sendTilEpostKnapp_Click);
            lagreSomPDF.Click += new RoutedEventHandler(lagreSomPDF_Click);
            kundeKnapp.Click += new RoutedEventHandler(kundeKnapp_Click);
            kummerKnapp.Click += new RoutedEventHandler(kummerKnapp_Click);
            
            forslagListBoks.DataContext = null;
            forslagListBoks.DataContext = corianForslagService.ForslagSomVises;

            totalprisLabel.DataContext = null;
            totalprisLabel.DataContext = corianForslagService.ForslagSomVises;

            totalprisIScrollLabel.DataContext = null;
            totalprisIScrollLabel.DataContext = corianForslagService.ForslagSomVises;


        }

        void kummerKnapp_Click(object sender, RoutedEventArgs e)
        {
            corianForslagService.leggTilKummer(this);
        }

   

       

        void annetTilvalgKnapp_Click(object sender, RoutedEventArgs e)
        {
            corianForslagService.leggTilAnnetTilvalg(this);
        }

        void kundeKnapp_Click(object sender, RoutedEventArgs e)
        {
            corianForslagService.endreKundeInfo(this as FrameworkElement);
        }

        void lagreSomPDF_Click(object sender, RoutedEventArgs e)
        {
            if (!klarTilPrint()) return;

            bool liggendeAnbefalt = (produktColumn.ActualWidth > 435.0);

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
                KundeViserKontroll kundeKontroll = new KundeViserKontroll(corianForslagService.ForslagSomVises, true);
                string pdfUrl = Printer.PrintSom(kundeKontroll, scrollKontroll, true, dlg.FileName, knapperGridColumn, null, liggende);

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

            bool liggendeAnbefalt = (produktColumn.ActualWidth > 435.0);

            PDFvalgVindu pdfValgVindu = new PDFvalgVindu(liggendeAnbefalt);
            pdfValgVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(this);
            pdfValgVindu.ShowDialog();

            if (pdfValgVindu.DialogResult != true) return;
            bool liggende = (bool)pdfValgVindu.liggendeBoks.IsChecked;
            

            KundeViserKontroll kundeKontroll = new KundeViserKontroll(corianForslagService.ForslagSomVises, true);
            string pdfUrl = Printer.PrintSom(kundeKontroll, scrollKontroll, true, "", knapperGridColumn, null, liggende);

            if (File.Exists(pdfUrl))
                Hjelpeklasser.EpostSender.SendEpost(pdfUrl, "Prisforslag", corianForslagService.ForslagSomVises.Epost);
        }

        void print_Click(object sender, RoutedEventArgs e)
        {
            if (!klarTilPrint()) return;

            if (produktColumn.ActualWidth > 435.0)
                MessageBox.Show("For dette prisforslaget er det anbefalt å bruke liggende format.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Information);

            KundeViserKontroll kundeKontroll = new KundeViserKontroll(corianForslagService.ForslagSomVises, false);
            Printer.PrintSom(kundeKontroll, scrollKontroll, false, "", knapperGridColumn, null, false);
        }

        bool klarTilPrint()
        {
            if (corianForslagService == null || corianForslagService.ForslagSomVises == null)
                return false;

            if (string.IsNullOrEmpty(corianForslagService.ForslagSomVises.Navn))
            {
                MessageBox.Show("Du må legge til kundeinformasjon før du kan gå videre.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            produktColumn.Width = double.NaN;
            forslagListBoks.UpdateLayout();
            return true;
        }

   

        void nyCorianplateKnapp_Click(object sender, RoutedEventArgs e)
        {
            corianForslagService.leggTilCorianprodukter(this as FrameworkElement);
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void corianItemButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem clickedItem = Corinor.Hjelpeklasser.VisualTree.getParentKontroll<ListViewItem>(sender as DependencyObject);
            ProduktPresentasjon produkt = forslagListBoks.ItemContainerGenerator.ItemFromContainer(clickedItem) as ProduktPresentasjon;
            corianForslagService.ForslagSomVises.Remove(produkt);
        }

        void db_ConnectionOK(object sender, EventArgs e)
        {
            toolbar.IsEnabled = true;
        }

        void db_ConnectionLost(object sender, EventArgs e)
        {
            toolbar.IsEnabled = false;
        }
    }
}
