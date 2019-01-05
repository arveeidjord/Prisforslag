using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Corinor.Modell.CorianProdukt
{
    [Serializable]
    public class Farge : INotifyPropertyChanged
    {


        //public string FargeTittel { get; set; }
        private string _fargeTittel = "";
        public string FargeTittel
        {
            get { return _fargeTittel; }
            set { _fargeTittel = value; OnPropertyChanged("FargeTittel"); }
        }

        //public string PrisgruppeNavn{get; private set;}
        public FargeBeholder FargeBeholderSomFargenErI { get; private set; }

        public List<Merknad> FargeMerknader { get; private set; }

        public string SortingTittel { get { return this.ToString(); } }

        private string _setMerknader = "";
        public string SetMerknader
        {
            get {
                return _setMerknader;
                //if (FargeMerknader == null) return "";
                //else 
                //{
                //    string s = "";
                //    foreach (Merknad m in FargeMerknader)
                //        s += m.MerknadMerke + ";";

                //    return s.TrimEnd(';');
                //}
            }

            set
            {
                _setMerknader = value;
                OnPropertyChanged("SetMerknader");
            }

        }

        public Farge(string fargeTittel, FargeBeholder fargeBeholderSomFargenErI)
        {
            FargeMerknader = new List<Merknad>();
            this.FargeTittel = fargeTittel;
            //this.PrisgruppeNavn = prisgruppeNavn;
            this.FargeBeholderSomFargenErI = fargeBeholderSomFargenErI;
        }

        public override string ToString()
        {
            //if (FargeBeholderSomFargenErI != null)
            //    return FargeTittel + " (" + FargeBeholderSomFargenErI.PrisgruppeNavn + ")";
            //else
                return FargeTittel;

            //return PrisgruppeNavn + ": " + FargeTittel;
            //return base.ToString();
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
