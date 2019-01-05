using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Modell.CorianProdukt;
using System.ComponentModel;

namespace Corinor.Modell
{
    [Serializable]
    public class Produkt : INotifyPropertyChanged
    {
        public DelingType Deling { get; set; }

        public enum DelingType
        {
            CorianProdukt= 0,
            CorianKum = 1,
            CorianKumHeltreKum = 2,
            HeltreKum = 3,

            Corian = 4,
            Heltre = 5,
            Begge = 6,
            //Begge = 5,
        }

        public string DelingString
        {
            get
            {
                if (Deling == DelingType.CorianProdukt) return "Corian produkt"; //0
                else if (Deling == DelingType.CorianKum) return "Corian kum"; //1
                else if (Deling == DelingType.CorianKumHeltreKum) return "Corian og heltre kum"; //2
                else if (Deling == DelingType.HeltreKum) return "Heltre kum"; //3
                else return "-";


            }
        }

        public virtual int DelingInt
        {

            //<ComboBoxItem Content="Corian produkt" />
            //<ComboBoxItem Content="Corian kum" />
            //<ComboBoxItem Content="Corian og heltre kum" />
            //<ComboBoxItem Content="Heltre kum" />

            get { return (int)Deling; }
            set
            {
                if (value != (int)Deling)
                {
                    if (value == 0) Deling = DelingType.CorianProdukt;
                    else if (value == 1) Deling = DelingType.CorianKum;
                    else if (value == 2) Deling = DelingType.CorianKumHeltreKum;
                    else if (value == 3) Deling = DelingType.HeltreKum;
                    else Deling = DelingType.CorianProdukt;

                    //OnPropertyChanged("DelingInt");
                    OnPropertyChanged("DelingString"); //Rett med: DelingString
                }
            }
        }


        //public string PrisPerString
        //{
        //    get
        //    {
        //        if (PrisPer == PerPrisType.perLøpemeter) return "Per løpemeter";
        //        else return "Per stykk";
        //    }
        //}

        //public int PrisPerInt
        //{
        //    get
        //    {
        //        return (int)PrisPer;
        //    }

        //    set
        //    {
        //        if (value == 1) PrisPer = PerPrisType.perLøpemeter;
        //        else PrisPer = PerPrisType.perAntall;
        //        OnPropertyChanged("PrisPerString"); //Rett med PrisPerString!!!
        //    }
        //}


        //public PerPrisType PrisPer { get; set; }

        //public enum PerPrisType
        //{
        //    perAntall = 0,
        //    perLøpemeter = 1,
        //}


        public Produkt(DelingType deling)
        { 
            this.Deling = deling; 
        
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
