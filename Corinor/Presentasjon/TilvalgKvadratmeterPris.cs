using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Kontroller;
using System.Windows.Controls;

namespace Corinor.Presentasjon
{
    public class TilvalgKvadratmeterPris : KontrollProduktPresentasjon
    {
        public override string Benevning
        {
            get { return "m²"; }
        }

        public override double AntallEnheterPresentasjon
        {
            get { return Math.Round((AntallEnheter / 1000000.0), 2); }
            set { AntallEnheter = value * 1000000.0; }
        }

        public override double TotalPris
        {
            get { return Math.Round((double)(PrisPerEnhet * (AntallEnheter / 1000000.0)),2); }
        }

        public TilvalgKvadratmeterPris(string tilvalgNavn, double prisPerEnhet)
            : base(tilvalgNavn, prisPerEnhet)
        {
            if (Kontroll != null && Kontroll is PrisKvadratmeterKontroll)
            {
                (Kontroll as PrisKvadratmeterKontroll).totalPris.DataContext = this;
                (Kontroll as PrisKvadratmeterKontroll).lengdeTekstboks.TextChanged += Tekstboks_TextChanged;
                (Kontroll as PrisKvadratmeterKontroll).breddeTekstboks.TextChanged += Tekstboks_TextChanged;

                (Kontroll as PrisKvadratmeterKontroll).lengdeTekstboks.Text = "1000";
                (Kontroll as PrisKvadratmeterKontroll).breddeTekstboks.Text = "1000";

                (Kontroll as PrisKvadratmeterKontroll).tittelLabel.Text = tilvalgNavn;
                (Kontroll as PrisKvadratmeterKontroll).prisPerEnhetLabel.Content = prisPerEnhet;
            }
        }

        protected override System.Windows.FrameworkElement opprettKontroll()
        {
            PrisKvadratmeterKontroll kontroll = new PrisKvadratmeterKontroll();
            return kontroll;
        }
            
        void Tekstboks_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lengde = 0;
            int bredde = 0;

            if (Kontroll != null && Kontroll is PrisKvadratmeterKontroll)
            {
                string l = (Kontroll as PrisKvadratmeterKontroll).lengdeTekstboks.Text;
                string b = (Kontroll as PrisKvadratmeterKontroll).breddeTekstboks.Text;

                int.TryParse(l, out lengde);
                int.TryParse(b, out bredde);

            }


            AntallEnheter = lengde * bredde;
        }
    }
}
