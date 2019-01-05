using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Corinor.Modell.CorianProdukt
{
    [Serializable]
    public class CorianPrisgruppeProdukt : INotifyPropertyChanged
    {
        //public string Navn { get; set; }
        FargeBeholder _farger;

        public FargeBeholder Farger { 
            get { return _farger; } 
            set { _farger = value; } 
        }
        //public PrisgruppeEntry Farger { get; set; }

        public double Pris
        {
            get
            {
                if (prisType == PrisType.Multipliser && Avhengighet != null)
                    return Avhengighet.Pris * Prisgrunnlag;
                else if (prisType == PrisType.Adder && Avhengighet != null)
                    return Avhengighet.Pris + Prisgrunnlag;
                else
                    return Prisgrunnlag;
            }
        }


        private double _prisgrunnlag = 0;
        public double Prisgrunnlag
        {
            get { return _prisgrunnlag; }
            set {
                if (value != _prisgrunnlag)
                {
                    _prisgrunnlag = value;
                    OnPropertyChanged("Prisgrunnlag");
                    OnPropertyChanged("Pris");
                }
            }
        }

        public CorianPrisgruppeProdukt Avhengighet { get; set; }


        public PrisType prisType { get; private set; }

        public string PrisTypeString 
        {
            get 
            {
                if (prisType == PrisType.Adder) return "+";
                else if (prisType == PrisType.Multipliser) return "x";
                else return "";
            }
        }

        public int PrisTypeInt 
        { 
            get 
            { 
                return (int) prisType; 

            } 
            set 
            {
                if (value == 0) prisType = PrisType.Adder;
                else if (value == 1) prisType = PrisType.Multipliser;
                else prisType = PrisType.Normal;
                OnPropertyChanged("PrisTypeInt");
                OnPropertyChanged("PrisTypeString");
            }
        
        }

        public enum PrisType 
        { 
            Normal = -1,
            Adder = 0,
            Multipliser = 1,
        }

        public CorianPrisgruppeProdukt(double prisgrunnlag, CorianPrisgruppeProdukt avhengighet, PrisType pristype, FargeBeholder farger)
        {
            this.Farger = farger;
            this.Prisgrunnlag = prisgrunnlag;
            this.Avhengighet = avhengighet;
            this.prisType = pristype;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //public override string ToString()
        //{
        //    return Navn;
        //}
    }
}
