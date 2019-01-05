using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Kontroller;
using System.Windows;
using System.Windows.Controls;

namespace Corinor.Presentasjon
{
    public class TilvalgGruppePresentasjon : KontrollProduktPresentasjon
    {
        public KontrollProduktPresentasjon AktivtTilvalg { get; private set; }
        KontrollProduktPresentasjon[] tilvalg;

        public TilvalgGruppePresentasjon(string tilvalgNavn, double prisPerEnhet, KontrollProduktPresentasjon[] tilvalg)
            : base(tilvalgNavn, prisPerEnhet)
        {
            this.tilvalg = tilvalg;

            if (Kontroll != null && Kontroll is GruppeKontroll)
            {
                GruppeKontroll gruppeKontroll = Kontroll as GruppeKontroll;
                gruppeKontroll.tittelLabel.Text = tilvalgNavn;

                gruppeKontroll.valgKombo.Items.Clear();

                if (tilvalg != null)
                    foreach (KontrollProduktPresentasjon _tilvalg in tilvalg)
                        gruppeKontroll.valgKombo.Items.Add(_tilvalg);

                gruppeKontroll.valgKombo.SelectionChanged += valgKombo_SelectionChanged;

                if (gruppeKontroll.valgKombo.Items.Count > 0)
                    gruppeKontroll.valgKombo.SelectedIndex = 0;
            }
        }

        void valgKombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Kontroll == null || !(Kontroll is GruppeKontroll)) return;
           
            GruppeKontroll gruppeKontroll = Kontroll as GruppeKontroll;
            ComboBox cmb = sender as ComboBox;

            if (cmb != null && cmb.SelectedItem != null && cmb.SelectedItem is KontrollProduktPresentasjon)
            {
                AktivtTilvalg = (cmb.SelectedItem as KontrollProduktPresentasjon);
                gruppeKontroll.innhold.Content = (cmb.SelectedItem as KontrollProduktPresentasjon).Kontroll;
            }
            else
            {
                AktivtTilvalg = null;
                gruppeKontroll.innhold.Content = null;
            }

          
        }
        
        
        protected override FrameworkElement opprettKontroll()
        {
            GruppeKontroll kontroll = new GruppeKontroll();





            return kontroll;

            //kontroll.totalPris.DataContext = this;
            //kontroll.antallTekstBoks.Text = "1";
            //kontroll.tittelLabel.Content = TilvalgNavn;
            //kontroll.beskrivelseLabel.Content = "Antall";

            
        }
    }
}
