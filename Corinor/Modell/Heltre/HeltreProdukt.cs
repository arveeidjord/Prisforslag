using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Corinor.Modell.Heltre
{
    [Serializable]
    public class HeltreProdukt : Produkt, INotifyPropertyChanged
    {




        //public string Treslag { get; set; } //Bjørk
        //public string Tykkelse { get; set; } //30 MM Fingerskjøtet
        //public string Type { get; set; } //Benkeplate /Halv hjørneplate
        //public string DybdeintervallStørrelse { get; set; }//Intill 300 / 900x900
        //public double Pris { get; set; }

        private string _treslag = "";
        private string _tykkelse = "";
        private string _type = "";
        private string _størrelse = "";
        private double _pris = 0;

        public string Treslag
        {
            get { return _treslag; }
            set 
            { 
                _treslag = value;
                OnPropertyChanged("Treslag"); 
            }
        }

        public string Tykkelse
        {
            get { return _tykkelse; }
            set
            {
                _tykkelse = value;
                OnPropertyChanged("Tykkelse");
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public string DybdeintervallStørrelse
        {
            get { return _størrelse; }
            set
            {
                _størrelse = value;
                OnPropertyChanged("DybdeintervallStørrelse");
            }
        }

        public double   Pris
        {
            get { return _pris; }
            set
            {
                _pris = value;
                OnPropertyChanged("Pris");
            }
        }



        public string PrisPerString { 
            get {
                if (PrisPer == PerPrisType.perLøpemeter) return "Per løpemeter";
                else return "Per stykk";
            } 
        }

        public int PrisPerInt 
        {
            get {
                return (int)PrisPer;
            }

            set 
            {
                if (value == 1) PrisPer = PerPrisType.perLøpemeter;
                else PrisPer = PerPrisType.perAntall;
                OnPropertyChanged("PrisPerString"); //Rett med PrisPerString!!!
            }
        }


        public PerPrisType PrisPer { get; set; }

        public enum PerPrisType
        { 
            perAntall = 0,
            perLøpemeter = 1,
        }

        public HeltreProdukt(string tykkelse, string treslag, string type, string dybdeintervallStørrelse, double pris, PerPrisType prisPer)
            :base(DelingType.Heltre)
        {
            this.Treslag = treslag;
            this.Tykkelse = tykkelse;
            this.Type = type;
            this.DybdeintervallStørrelse = dybdeintervallStørrelse;
            this.Pris = pris;
            this.PrisPer = prisPer;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
