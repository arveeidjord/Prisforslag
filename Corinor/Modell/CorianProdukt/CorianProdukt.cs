using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Corinor.Modell.CorianProdukt
{
    [Serializable]
    public class CorianProdukt : Produkt
    {
        private CorianPrisgruppeProdukt _prisgruppe = null;
        public CorianPrisgruppeProdukt Prisgruppe
        {
            get { return _prisgruppe; }
            set
            {
                if (value != _prisgruppe)
                {
                    _prisgruppe = value;
                    OnPropertyChanged("Prisgruppe");
                }
            }
        }

        public CorianPrisgruppeProdukt[] Prisgrupper { get; set; }

        private string _navn = "";
        public string Navn
        {
            get { return _navn; }
            set
            {
                if (value != _navn)
                {
                    _navn = value;
                    OnPropertyChanged("Navn");
                }
            }
        }

        private string _produktKategori = "";
        public string ProduktKategori
        {
            get { return _produktKategori; }
            set
            {
                if (value != _produktKategori)
                {
                    _produktKategori = value;
                    OnPropertyChanged("ProduktKategori");
                }
            }
        }

        public string PrisPerString
        {
            get
            {
                if (PrisPer == PerPrisType.perLøpemeter) return "Per løpemeter";
                else return "Per stykk";
            }
        }

        public int PrisPerInt
        {
            get
            {
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

       

        public CorianProdukt(string produktKategori, string navn, DelingType deling)
            :base(deling)
        {
            Prisgrupper = new CorianPrisgruppeProdukt[30];
           
            Navn = navn;
            ProduktKategori = produktKategori;
        }
    }
}
