using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Kontroller;
using System.Windows;
using System.Windows.Controls;

namespace Corinor.Presentasjon
{
    public class LøpemeterprisXprosent : KontrollProduktPresentasjon
    {
        protected double prosent = 1;

        public LøpemeterprisXprosent(string produktnavn, double prosent) //double løpemeterpris
            :base(produktnavn, 0.0)
        {
            this.prosent = prosent;

            if (Kontroll != null && Kontroll is PrisLøpemeterprisKontroll)
            {
                (Kontroll as PrisLøpemeterprisKontroll).prosentLabel.Content = "x " + prosent;
                (Kontroll as PrisLøpemeterprisKontroll).tittelLabel.Content = produktnavn;
            }

        }

        protected override FrameworkElement opprettKontroll()
        {
            PrisLøpemeterprisKontroll kontroll = new PrisLøpemeterprisKontroll();
            kontroll.løpemeterprisTekstboks.TextChanged += new System.Windows.Controls.TextChangedEventHandler(løpemeterprisTekstboks_TextChanged);
            kontroll.antallTekstBoks.TextChanged += new TextChangedEventHandler(antallTekstBoks_TextChanged);
            kontroll.antallTekstBoks.Text = "1";
            
            //kontroll.
            kontroll.totalPris.DataContext = this;
            return kontroll;
        }

    

        protected virtual void løpemeterprisTekstboks_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tekstboks = sender as TextBox;
            if (tekstboks == null) return;

            double løpemeterpris = 0.0;
            double.TryParse(tekstboks.Text, out løpemeterpris);
            PrisPerEnhet = løpemeterpris * prosent;
        }

        void antallTekstBoks_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tekstboks = sender as TextBox;
            if (tekstboks == null) return;

            double antall = 0.0;
            double.TryParse(tekstboks.Text, out antall);
            AntallEnheter = antall;
        }
    }
}
