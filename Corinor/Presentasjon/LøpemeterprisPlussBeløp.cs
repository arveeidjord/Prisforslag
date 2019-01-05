using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Corinor.Kontroller;

namespace Corinor.Presentasjon
{
    public class LøpemeterprisPlussBeløp : LøpemeterprisXprosent
    {

        public LøpemeterprisPlussBeløp(string produktnavn, double tilleggsBeløp)
            : base(produktnavn, tilleggsBeløp)
        {
            if (Kontroll != null && Kontroll is PrisLøpemeterprisKontroll)
            {
                (Kontroll as PrisLøpemeterprisKontroll).prosentLabel.Content = "+ " + prosent;
            }
        }

        protected override void løpemeterprisTekstboks_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tekstboks = sender as TextBox;
            if (tekstboks == null) return;

            double løpemeterpris = 0.0;
            double.TryParse(tekstboks.Text, out løpemeterpris);
            PrisPerEnhet = løpemeterpris + prosent; // prosent brukes som et beløp
        }
    }
}
