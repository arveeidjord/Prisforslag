using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Corinor.Modell.CorianProdukt
{
    [Serializable]
    public class FargeBeholder : INotifyPropertyChanged
    {
        //public static ObservableCollection<Merknad> MerknaderStatisk{ get; set; }

        ////For serialisering
        //public ObservableCollection<Merknad> MerknaderStatisk {
        //    get
        //    {
        //        if (FargeBeholder._merknader == null) FargeBeholder._merknader = new ObservableCollection<Merknad>(); 
        //        return _merknader; 
        //    }
        //    set {
        //        FargeBeholder._merknader = value;
        //    }
        //}

        private bool _visible = false;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                OnPropertyChanged("Visible");
            }
        }


        public string PrisgruppeNavn { get; set; }
        public ObservableCollection<Farge> Farger {get; private set;}

        public FargeBeholder(string prisgruppeNavn)
        {
            this.PrisgruppeNavn = prisgruppeNavn;
            Farger = new ObservableCollection<Farge>();
        }

        public override string ToString()
        {
            return PrisgruppeNavn;
            //return base.ToString();
        }


        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        //public string PrisgruppeNavn { get; private set; }
        //public string Navn { get; private set; }

        //public PrisgruppeEntry(string prisgruppeNavn, string navn)
        //{
        //    this.PrisgruppeNavn = prisgruppeNavn;
        //    this.Navn = navn;
        //}

    }
}
