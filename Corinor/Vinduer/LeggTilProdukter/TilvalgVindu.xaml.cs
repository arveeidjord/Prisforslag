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
using Corinor.Kontrollbehandling;
using Corinor.DataAksess;
using Corinor.Hjelpeklasser;
using Corinor.Presentasjon;
using Corinor.Presentasjon.Forslag;
using Corinor.Modell.Tilvalg;
using System.Collections.ObjectModel;
using Corinor.Modell;

namespace Corinor.Vinduer
{
    /// <summary>
    /// Interaction logic for HeltraplateTilvalgVindu.xaml
    /// </summary>
    public partial class TilvalgVindu : Window
    {
        public ObservableCollection<TilvalgGruppe> Liste { get; private set; }

        TilvalgHeltreplateService tilvalgService;

        public KontrollProduktPresentasjon TilvalgSomEndres
        {
            get;
            private set;
        }

        public TilvalgVindu(DataAksess2 db, ForslagPresentasjon forslag, Corinor.Modell.Produkt.DelingType deling)
        {
            InitializeComponent();
            
            tilvalgService = new TilvalgHeltreplateService(forslag);
            
            tilvalgKombo.SelectionChanged += tilvalgKombo_SelectionChanged;
            LeggTilKnapp.Click += new RoutedEventHandler(LeggTilKnapp_Click);
            avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);

           
            Liste = new ObservableCollection<TilvalgGruppe>();

            foreach (TilvalgGruppe p in db.Produktbeholder.Tilvalgliste)
                if (p.Deling == deling || p.Deling == Produkt.DelingType.Begge)
                    Liste.Add(p);

            tilvalgKombo.DataContext = this;

        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            TilvalgSomEndres = null;
            this.Close();
        }

        void LeggTilKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (TilvalgSomEndres != null && TilvalgSomEndres is TilvalgGruppePresentasjon)
                TilvalgSomEndres = (TilvalgSomEndres as TilvalgGruppePresentasjon).AktivtTilvalg;

            DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Modell.Tilvalg.TilvalgGruppe[] tilvalgListe = tilvalgService.getAlleTilvalg();
            //KontrollHjelper.oppdaterKombo(tilvalgKombo, tilvalgListe);
        }


        void tilvalgKombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox cmb = sender as ListBox;
            if (cmb.SelectedItem == null || !(cmb.SelectedItem is TilvalgGruppe))
                return;

            TilvalgSomEndres = tilvalgService.getTilvalg(cmb.SelectedItem as TilvalgGruppe);
            if (TilvalgSomEndres == null) tilvalgInnhold.Content = null;
            else tilvalgInnhold.Content = TilvalgSomEndres.Kontroll;
            LeggTilKnapp.IsEnabled = true;
            
        }





    }
}
