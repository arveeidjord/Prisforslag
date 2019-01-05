using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.DataAksess;
using System.Windows;
using System.ComponentModel;

namespace Corinor.Presentasjon
{
    public class HeltreplatePresentasjon : ProduktPresentasjon
    {
        private string _treslag;
        private string _benkeplateType;
        private string _tykkelse;
        private string _størrelse;
        private int _lengde;

        private bool prisPerLøpemeter = false;

        public override string Benevning
        {
            get
            {
                if (prisPerLøpemeter)
                    return "mm";
                else
                    return "stk.";
            }
        }

        public string Treslag 
        {
            get { return _treslag;}
            private set
            {
                _treslag = value;
                OnPropertyChanged("Treslag");
            }
        }

        public string BenkeplateType
        {
            get { return _benkeplateType; }
            private set
            {
                _benkeplateType = value;
                OnPropertyChanged("BenkeplateType");
            }
        }

        public string Tykkelse
        {
            get { return _tykkelse; }
            private set
            {
                _tykkelse = value;
                OnPropertyChanged("Tykkelse");
            }
        }

        public string Størrelse
        {
            get { return _størrelse; }
            private set
            {
                _størrelse = value;
                OnPropertyChanged("Størrelse");
            }
        }

        public int Lengde
        {
            get { return _lengde; }
            set
            {
                _lengde = value;
                OnPropertyChanged("Lengde");

                if (prisPerLøpemeter)
                    AntallEnheter = Math.Round(value / 1000.0, 2);
                else
                    AntallEnheter = value;
            }
        }

        public HeltreplatePresentasjon(string treslag, string benkeplateType, string tykkelse, string størrelse, int lengde, double pris, bool prisPerLøpemeter)
            : base(treslag, pris)
        {
            oppdaterPlate(treslag, benkeplateType, tykkelse, størrelse, lengde, pris, prisPerLøpemeter);
        }

        public void oppdaterPlate(string treslag, string benkeplateType, string tykkelse, string størrelse, int lengde, double pris, bool prisPerLøpemeter)
        {
            this.prisPerLøpemeter = prisPerLøpemeter;

            this.Treslag = treslag;
            this.BenkeplateType = benkeplateType;
            this.Tykkelse = tykkelse;
            this.Størrelse = størrelse;
            this.Lengde = lengde;
            this.PrisPerEnhet = pris;
            
        }
    }
}
