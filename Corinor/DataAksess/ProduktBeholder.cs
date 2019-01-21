using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Corinor.Modell.Tilvalg;
using Corinor.Modell.CorianProdukt;
using Corinor.Modell.Heltre;
using System.IO;

namespace Corinor.DataAksess
{
    [Serializable]
    public class ProduktBeholder
    {
        //For Corian
        public ObservableCollection<CorianProdukt> ProduktListe{get; private set;}
        public List<FargeBeholder> Prisgruppesamling { get; private set; }

        

        //For Heltre
        public ObservableCollection<HeltreProdukt> HeltreProduktliste { get; private set; }
        public ObservableCollection<TilvalgGruppe> Tilvalgliste { get; private set; }

        private ObservableCollection<Merknad> _merknader = null;
        public ObservableCollection<Merknad> Merknader { get { return _merknader; } set { _merknader = value;  } }

        public DateTime SistEndret { get; private set; }

        public ProduktBeholder()
        {
            
            ProduktListe = new ObservableCollection<CorianProdukt>();
            Prisgruppesamling = new List<FargeBeholder>();

            for (int i = 0; i < 30; i++)
                Prisgruppesamling.Add(new FargeBeholder("Prisgruppe " + (i+1)));

            HeltreProduktliste = new ObservableCollection<HeltreProdukt>();

            Tilvalgliste = new ObservableCollection<TilvalgGruppe>();
            Merknader = new ObservableCollection<Merknad>();

            SistEndret = DateTime.MinValue;
        }

        public bool saveData()
        {
            this.SistEndret = DateTime.Now;
            string url = Hjelpeklasser.GlobaleUrier.prislistefilUri();
            Hjelpeklasser.Serializer<ProduktBeholder> s = new Hjelpeklasser.Serializer<ProduktBeholder>();
            return s.serialize(url, this);
        }

        public static ProduktBeholder loadData(string url)
        {
            try
            {
                Hjelpeklasser.Serializer<ProduktBeholder> s = new Hjelpeklasser.Serializer<ProduktBeholder>();
                return s.deSerialize(url) as ProduktBeholder;
            }
            catch (Exception e)
            { 
                return null; 
            }
        }

        public static bool erPrislistefil(string url)
        {
            ProduktBeholder pb = null;
            try
            {
                Hjelpeklasser.Serializer<ProduktBeholder> s = new Hjelpeklasser.Serializer<ProduktBeholder>();
                pb = s.deSerialize(url) as ProduktBeholder;
            }
            catch (Exception e)
            {
                pb = null;
            }

            if (pb == null) return false;
            else return true;
            

        }
    }
}
