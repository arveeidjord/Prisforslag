using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Corinor.Kontroller;
using System.Windows.Controls;

namespace Corinor.Presentasjon
{
    public class TilvalgEgendefinertPris : KontrollProduktPresentasjon
    {

        public TilvalgEgendefinertPris(string navn, double prisPerEnhet)
            : base(navn, prisPerEnhet)
        {
            if (Kontroll != null && Kontroll is PrisEgendefinertKontroll)
            {
                (Kontroll as PrisEgendefinertKontroll).totalPris.DataContext = this;
                (Kontroll as PrisEgendefinertKontroll).tittelLabel.Text = navn;

                //(Kontroll as PrisEgendefinertKontroll).prisPerEnhetLabel.Text = prisPerEnhet.ToString();

                (Kontroll as PrisEgendefinertKontroll).prisPerEnhetTekstboks.TextChanged += prisPerEnhetTekstboks_TextChanged;
                (Kontroll as PrisEgendefinertKontroll).antallEnheterTekstBoks.TextChanged += antallEnheterTekstboks_TextChanged;

                (Kontroll as PrisEgendefinertKontroll).prisPerEnhetTekstboks.Text = prisPerEnhet.ToString();
                (Kontroll as PrisEgendefinertKontroll).antallEnheterTekstBoks.Text = "1";
            }
        }

  


        protected override FrameworkElement opprettKontroll()
        {
            PrisEgendefinertKontroll kontroll = new PrisEgendefinertKontroll();
            return kontroll;
        }

        void antallEnheterTekstboks_TextChanged(object sender, TextChangedEventArgs e)
        {
            int antall = 0;
            int.TryParse((sender as TextBox).Text, out antall);

            AntallEnheter = antall;
        }

        void prisPerEnhetTekstboks_TextChanged(object sender, TextChangedEventArgs e)
        {
            double pris = 0;
            double.TryParse((sender as TextBox).Text, out pris);

            PrisPerEnhet = pris;
        }
    }
}
