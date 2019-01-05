using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Kontroller;
using System.Windows.Controls;

namespace Corinor.Presentasjon
{
    public class TilvalgEnkelPrisTusendel : TilvalgEnkelPris
    {

        public override string Benevning
        {
            get { return "mm"; }
        }

        public override double TotalPris
        {
            get { return Math.Round((PrisPerEnhet * (AntallEnheter / 1000.0)), 2); }
        }


        public TilvalgEnkelPrisTusendel(string tilvalgNavn, double pris)
            : base(tilvalgNavn, pris)
        {
            if (Kontroll != null && Kontroll is PrisPerAntallTilvalgKontroll)
            {
                (Kontroll as PrisPerAntallTilvalgKontroll).antallEnheterTekstBoks.Text = "1000";
                (Kontroll as PrisPerAntallTilvalgKontroll).antallEnheterTekstBoks.TextChanged += antallEnheter_TextChanged;

                (Kontroll as PrisPerAntallTilvalgKontroll).prisPerEnhetTekstLabel.Content = "Pris per løpemeter";
                (Kontroll as PrisPerAntallTilvalgKontroll).beskrivelseLabel.Content = "Antall løpemeter (mm)";

            }
            AntallEnheter = 1000;
        }

        void antallEnheter_TextChanged(object sender, TextChangedEventArgs e)
        {
            int antall = 0;
            int.TryParse((sender as TextBox).Text, out antall);

            AntallEnheter = antall;
        }
    }
}
