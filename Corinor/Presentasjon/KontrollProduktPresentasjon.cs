using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace Corinor.Presentasjon
{
    public abstract class KontrollProduktPresentasjon : ProduktPresentasjon // INotifyPropertyChanged // : ProduktPresentasjon
    {
        public FrameworkElement Kontroll { get; protected set; }

        public KontrollProduktPresentasjon(string tilvalgNavn, double prisPerEnhet)
            : base(tilvalgNavn, prisPerEnhet)
        {
            Kontroll = opprettKontroll();
        }

        protected abstract FrameworkElement opprettKontroll();

    }
}
