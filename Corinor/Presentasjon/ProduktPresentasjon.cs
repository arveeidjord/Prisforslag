using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Corinor.Modell.CorianProdukt;

namespace Corinor.Presentasjon
{
    public class ProduktPresentasjon : INotifyPropertyChanged
    {
        private string _produktNavn = "";
        private string _produktKommentar = "";
        private double _prisPerEnhet = 0;
        private double _antallEnheter = 0;

        //public List<Merknad> Merknader2 { get; set; }
        public Merknad[] Merknader3 { get; set; }

        public bool AvhengigAvTotalsum { get; protected set; }

        public virtual double TotalPris
        {
            get { return Math.Round((double)(PrisPerEnhet * AntallEnheter), 2); }
        }

        public virtual string Benevning
        {
            get { return "stk."; }
            //get { return ""; }

        }

        
        public double PrisPerEnhet
        {
            get { return _prisPerEnhet; }
            protected set
            {
                if (_prisPerEnhet == value)
                    return;

                _prisPerEnhet = value;
                OnPropertyChanged("PrisPerEnhet");
                OnPropertyChanged("TotalPris");
            }
        }

        public double AntallEnheter
        {
            get { return _antallEnheter; }
            set //protected
            {
                if (_antallEnheter == value)
                    return;

                _antallEnheter = value;
                OnPropertyChanged("AntallEnheter");
                OnPropertyChanged("TotalPris");
            }
        }

        public virtual double AntallEnheterPresentasjon
        {
            get { return AntallEnheter; }
            set { AntallEnheter = value; }
        }

        public string ProduktNavn
        {
            get { return _produktNavn; }
            set
            {
                if (_produktNavn == value)
                    return;

                _produktNavn = value;
                OnPropertyChanged("ProduktNavn");
            }
        }

        public string ProduktKommentar
        {
            get { return _produktKommentar; }
            set
            {
                if (_produktKommentar == value)
                    return;

                _produktKommentar = value;
                OnPropertyChanged("ProduktKommentar");
            }
        }

        public ProduktPresentasjon(string produktNavn, double prisPerEnhet)
        {
            ProduktNavn = produktNavn;
            PrisPerEnhet = prisPerEnhet;
            AvhengigAvTotalsum = false;
            AntallEnheter = 1;
        }

        public override string ToString()
        {
            return ProduktNavn;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

       

    }
}
