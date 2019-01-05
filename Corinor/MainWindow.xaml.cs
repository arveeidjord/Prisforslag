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
using Corinor.Presentasjon;
using Corinor.Kontroller;
using Corinor.Kontroller.Forslag;
using Corinor.Vinduer;
using Corinor.Presentasjon.Forslag;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Corinor.Kontrollbehandling;
using Corinor.DataAksess;
using Corinor.Vinduer.EndrePrislister;
using System.ComponentModel;

namespace Corinor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        HovedvinduService hovedvinduService;

        public MainWindow()
        {
            //############################################################
            // Debug
            //DateTime framtid = new DateTime(2012, 1, 1);
            //if (DateTime.Compare(framtid, DateTime.Now) < 0)
            //{
            //    MessageBox.Show("Denne prøveversjonen av programmet har gått ut på dato.\nDu må få tak i den endelige versjonen.\n\nProgrammet vil nå avslutte.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Stop);
            //    this.Close();
            //}
            //##############################################################

            InitializeComponent();

            hovedvinduService = new HovedvinduService(this, updateImage);
           

            nyttHeltreForslagKnapp.Click += nyttHeltreForslagKnapp_Click;
            nyttCorianForslagKnapp.Click += nyttCorianForslagKnapp_Click;
            prisbehandlingKnapp.Click += prisbehandlingKnapp_Click;

            //nyttHeltreForslagKnapp_Click(null, null);
            //nyttCorianForslagKnapp_Click(null, null);

            prislisteCorianEksport.Click += prislisteCorianEksport_Click;
            prislisteCorianImport.Click += prislisteCorianImport_Click;

            endrePrislisteCorianKnapp.Click += endreCorianprislisteKnapp_Click;
            endrePrislisteHeltreKnapp.Click += endreHeltreprislisteKnapp_Click;
            endreTilvalgCorianKnapp.Click += endreTilvalgKnapp_Click;

            hjelpKnapp.Click += new RoutedEventHandler(hjelpKnapp_Click);
            prislisteHjelpKnapp.Click += new RoutedEventHandler(prislisteHjelpKnapp_Click);

            updateImage.MouseUp += new MouseButtonEventHandler(updateImage_MouseUp);
            updateAvPaaKnapp.Click += new RoutedEventHandler(updateAvPaaKnapp_Click);

        }

        void updateAvPaaKnapp_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoUpdate = !Properties.Settings.Default.AutoUpdate;
            Properties.Settings.Default.Save();

            var autoUpdate = Properties.Settings.Default.AutoUpdate;
            updateImage.IsEnabled = autoUpdate;

            var sistUpdate = Corinor.Properties.Settings.Default.SisteUpdate; //.AddDays(-7);
            var d = sistUpdate.Date.Subtract(DateTime.Today).Days + 7;
            if (d < 0) d = 0;
            if (d > 7) d = 7;

            var antDagerTilSjekk = d;
 
            if (autoUpdate)
                hovedvinduService.setUploadImage(antDagerTilSjekk + " dager til neste sjekk av ny prisliste\n(Klikk for å se etter ny prisliste nå)", Corinor.Kontrollbehandling.HovedvinduService.BildeEnum.OK);            
            else
                hovedvinduService.setUploadImage("Automatisk oppdatering av prisliste er slått av.", Corinor.Kontrollbehandling.HovedvinduService.BildeEnum.autoupdateOff);
        }

        void updateImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            hovedvinduService.OppdaterPrisliste();
        }

        void prislisteHjelpKnapp_Click(object sender, RoutedEventArgs e)
        {
            string startupPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string url = Path.Combine(startupPath, "brukerveiledning_Prislistebehandling.pdf");
            if (!File.Exists(url))
            {
                MessageBox.Show("Finner ikke brukerveiledningen. Kontakt Corianor AS for mer informasjon.\n\nhttp://corinor.no", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            try
            {
                Process.Start(url);
            }
            catch (Exception)
            {
                MessageBox.Show("Klarte ikke å åpne fila:\n\n" + url);
            }

           
        }

        void hjelpKnapp_Click(object sender, RoutedEventArgs e)
        {

            string startupPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string url = Path.Combine(startupPath, "brukerveiledning.pdf");
            if (!File.Exists(url))
            {
                MessageBox.Show("Finner ikke brukerveiledningen. Kontakt Corianor AS for mer informasjon.\n\nhttp://corinor.no", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Process.Start(url);
            }
            catch (Exception)
            {
                MessageBox.Show("Klarte ikke å åpne fila:\n\n" + url);
            }

        }

        void endreTilvalgKnapp_Click(object sender, RoutedEventArgs e)
        {
            hovedvinduService.EndreTilvalg(this);
        }

        void endreHeltreprislisteKnapp_Click(object sender, RoutedEventArgs e)
        {
            hovedvinduService.EndreHeltrePriser(this);
        }

        void endreCorianprislisteKnapp_Click(object sender, RoutedEventArgs e)
        {

            hovedvinduService.EndreCorianPriser(this);
        }

        string prislistebehandlingPassord = "Toten";
        bool passordBeskyttetKnappÅpen = false;

        //Åpne contextmenu
        void prisbehandlingKnapp_Click(object sender, RoutedEventArgs e)
        {
            string passord = "";
            if (passordBeskyttetKnappÅpen) passord = prislistebehandlingPassord;
            else
                passord = Microsoft.VisualBasic.Interaction.InputBox("Det kreves passord for å gjøre endringer i prislisten. \n\nPassord:", "Corinor prisforslag", "");

            if ((!string.IsNullOrEmpty(passord) && passord == prislistebehandlingPassord))
            {
                passordBeskyttetKnappÅpen = true;
                Button but = (Button)sender;
                if (but == null || but.ContextMenu == null) return;

                if (!but.ContextMenu.IsOpen)
                {
                    but.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    but.ContextMenu.PlacementTarget = but;

                    but.ContextMenu.IsOpen = true;
                    updateAvPaaKnapp.IsChecked = Properties.Settings.Default.AutoUpdate;
                }
            }
            else if (!string.IsNullOrEmpty(passord))
                MessageBox.Show("Passordet du oppgav var feil!", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Error);

            
  
        }

        #region Import og Eksport knappemetoder

        void prislisteCorianEksport_Click(object sender, RoutedEventArgs e)
        {
            hovedvinduService.eksportCorianPrisliste();
        }

      

        void prislisteCorianImport_Click(object sender, RoutedEventArgs e)
        {
            bool res = hovedvinduService.importPrisliste();
            if (!res) return;

            hovedvinduService.loadPrisliste(this);
        }

        #endregion


        #region Nytt forslag knappemetoder

        void nyttHeltreForslagKnapp_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = hovedvinduService.nyHeltreTab(FindResource("FlatButton") as Style, this);
            tabKontroll.Items.Add(tab);
            tabKontroll.SelectedItem = tab;
        }

        void nyttCorianForslagKnapp_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = hovedvinduService.nyCorianTab(FindResource("FlatButton") as Style, this);
            tabKontroll.Items.Add(tab);
            tabKontroll.SelectedItem = tab;
        }

        #endregion

        internal void CloseTabItemButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = Hjelpeklasser.VisualTree.getParentKontroll<TabItem>(sender as Button);

            if (tab != null)
                tabKontroll.Items.Remove(tab);
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
       
      
    }
}
