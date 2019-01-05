using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Kontroller;
using System.Windows.Controls;
using System.Windows;

namespace Corinor.Presentasjon
{
    public class TilvalgEnkelPris : KontrollProduktPresentasjon
    {

        //public TilvalgEnkelPris(string tilvalgNavn, double pris)
        //    : this(tilvalgNavn, pris)
        //{
        //    //ProduktKommentar = kommentar;
        //}



        public TilvalgEnkelPris(string tilvalgNavn, double pris)
            : base(tilvalgNavn, pris)
        {
            AntallEnheter = 1;
            PrisPerEnhet = pris;
            if (Kontroll != null && Kontroll is PrisPerAntallTilvalgKontroll)
            {
                (Kontroll as PrisPerAntallTilvalgKontroll).antallEnheterTekstBoks.TextChanged += antallEnheter_TextChanged;
                (Kontroll as PrisPerAntallTilvalgKontroll).prisPerEnhetLabel.Content = pris;
            }
        }

        protected override FrameworkElement opprettKontroll()
        {
            PrisPerAntallTilvalgKontroll kontroll = new PrisPerAntallTilvalgKontroll();
            
            kontroll.totalPris.DataContext = this;
            kontroll.antallEnheterTekstBoks.Text = "1";
            kontroll.tittelLabel.Text = ProduktNavn;
            //kontroll.beskrivelseLabel.Content = "Antall";

            return kontroll;
        }

        void antallEnheter_TextChanged(object sender, TextChangedEventArgs e)
        {
            int antall = 0;
            int.TryParse((sender as TextBox).Text, out antall);

            AntallEnheter = antall;
        }
    }
}
