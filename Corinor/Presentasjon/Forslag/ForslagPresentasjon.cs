using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Corinor.Modell.CorianProdukt;

namespace Corinor.Presentasjon.Forslag
{
    public class ForslagPresentasjon : INotifyPropertyChanged
    {
        private double _totalpris = 0;

        public string Tittel { get; set; }
        public string Navn { get; set; }
        public string Adresse { get; set; }
        public string Postnummer { get; set; }
        public string Poststed { get; set; }
        public string Epost { get; set; }
        public string Telefonnummere { get; set; }
        public string Kommentar { get; set; }

        public double TotalPrisUtenAvhengige { get; protected set; }

        public double Totalpris
        {
            get { return Math.Round(_totalpris, 2); }
            protected set
            {
                if (_totalpris == value) return;
                _totalpris = value;
                OnPropertyChanged("Totalpris");
            }
        }

        public ReadOnlyObservableCollection<ProduktPresentasjon> Produkter { get { return _produkterReadOnly; } }
        protected ObservableCollection<ProduktPresentasjon> _produkter;
        private ReadOnlyObservableCollection<ProduktPresentasjon> _produkterReadOnly;

        public ForslagPresentasjon(string tittel)
        {
            _produkter = new ObservableCollection<ProduktPresentasjon>();
            _produkterReadOnly = new ReadOnlyObservableCollection<ProduktPresentasjon>(_produkter);
            _produkter.CollectionChanged += _heltreplater_CollectionChanged;

            Tittel = tittel;
        }


        public void Add(ProduktPresentasjon element)
        {
            if (element == null) return;

            _produkter.Add(element);
            element.PropertyChanged += element_PropertyChanged;
        }

        public void Remove(ProduktPresentasjon element)
        {
            if (element == null) return;

            element.PropertyChanged -= element_PropertyChanged;
            _produkter.Remove(element);
        }

        protected virtual void beregnTotalpris()
        {
            TotalPrisUtenAvhengige = getTotalsumUtenAvhengige();
            Totalpris = getListeSum();
        }


        protected internal double getListeSum()
        {
            double sum = 0;
            foreach (ProduktPresentasjon plate in _produkter)
                if (plate != null)
                    sum += plate.TotalPris;

            return sum;
        }

        protected internal double getTotalsumUtenAvhengige()
        {
            double sumUtenAvhengige = 0;
            foreach (ProduktPresentasjon plate in _produkter)
                if (plate != null)
                    if (!plate.AvhengigAvTotalsum)
                        sumUtenAvhengige += plate.TotalPris;

            return sumUtenAvhengige;
        }


        protected void _heltreplater_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            beregnTotalpris();
        }


        protected void element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "PrisPerEnhet" || e.PropertyName == "TotalPris")
            if (e.PropertyName == "TotalPris")
                beregnTotalpris();

        }

        public override string ToString()
        {
            return Tittel;
        }




        //[field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
