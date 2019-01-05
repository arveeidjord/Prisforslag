using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Corinor.Presentasjon.Forslag
{
    public class HeltreForslagPresentasjon : ForslagPresentasjon
    {
        static int antallHeltreForslag = 0;

        
        public ReadOnlyObservableCollection<KontrollProduktPresentasjon> Tilvalg { get { return _tilvalgReadOnly; } }
        private ObservableCollection<KontrollProduktPresentasjon> _tilvalg;
        private ReadOnlyObservableCollection<KontrollProduktPresentasjon> _tilvalgReadOnly;

        public HeltreForslagPresentasjon()
           : base(string.Format("Heltre prisforslag {0}", ++antallHeltreForslag))
        {
            _tilvalg = new ObservableCollection<KontrollProduktPresentasjon>();
            _tilvalgReadOnly = new ReadOnlyObservableCollection<KontrollProduktPresentasjon>(_tilvalg);
            _tilvalg.CollectionChanged += _heltreplater_CollectionChanged;
        }

        public void AddTilvalg(KontrollProduktPresentasjon element)
        {
            if (element == null) return;

            _tilvalg.Add(element);
            element.PropertyChanged += element_PropertyChanged;
        }

        internal void RemoveTilvalg(KontrollProduktPresentasjon element)
        {
            if (element == null) return;

            element.PropertyChanged -= element_PropertyChanged;
            _tilvalg.Remove(element);
        }

        protected override void beregnTotalpris()
        {
            double totalPrisUtenAvhengige = getTotalsumUtenAvhengige();
            double totalPris = getListeSum();

            foreach (KontrollProduktPresentasjon plate in _tilvalg)
                if (plate != null)
                {
                    if (!plate.AvhengigAvTotalsum)
                        totalPrisUtenAvhengige += plate.TotalPris;

                    totalPris += plate.TotalPris;
                }

            TotalPrisUtenAvhengige = totalPrisUtenAvhengige;
            Totalpris = totalPris;
        }

        internal double getSumTilvalg()
        {
            double sumTilvalg = 0;
            foreach (KontrollProduktPresentasjon plate in _tilvalg)
                if (plate != null)
                    sumTilvalg += plate.TotalPris;

            return sumTilvalg;
        }

        
       



    





       
    }
}
