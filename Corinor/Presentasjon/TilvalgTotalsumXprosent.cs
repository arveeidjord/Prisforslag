using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Corinor.Kontroller;
using Corinor.Presentasjon.Forslag;

namespace Corinor.Presentasjon
{
    public class TilvalgTotalsumXprosentTilvalg : KontrollProduktPresentasjon
    {
        private double prosent = 1;

        private double grunnlag = -1;
        private double forslagetsTotalpris = 0;
        private double mistePris = -1;

        ForslagPresentasjon totalsumForslag;

        public TilvalgTotalsumXprosentTilvalg(string tilvalgNavn, double prosent, ForslagPresentasjon totalsumForslag, double minstepris)
            : base(tilvalgNavn, 0)
        {
            AvhengigAvTotalsum = true;

            this.prosent = prosent;
            this.mistePris = minstepris;
            this.forslagetsTotalpris = totalsumForslag.TotalPrisUtenAvhengige;
            
            this.totalsumForslag = totalsumForslag;
            this.totalsumForslag.PropertyChanged += totalsumForslag_PropertyChanged;
            
            beregnPris(forslagetsTotalpris);
        }

        private void beregnPris(double forslagetsTotalpris)
        {
            if (forslagetsTotalpris == grunnlag) return;

            double nyPrisPerEnhet =((forslagetsTotalpris) * prosent);
            grunnlag = nyPrisPerEnhet;
            
            if (mistePris > nyPrisPerEnhet)
                PrisPerEnhet = mistePris;
            else
                PrisPerEnhet = nyPrisPerEnhet;
        }

        void totalsumForslag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Totalpris" || sender == null || !(sender is ForslagPresentasjon))
                return;

            forslagetsTotalpris = (sender as ForslagPresentasjon).TotalPrisUtenAvhengige;
            beregnPris(forslagetsTotalpris);
        }


        protected override System.Windows.FrameworkElement opprettKontroll()
        {
            PrisKontroll kontroll = new PrisKontroll();
            kontroll.totalPris.DataContext = this;
            return kontroll;
        }

    }
}
