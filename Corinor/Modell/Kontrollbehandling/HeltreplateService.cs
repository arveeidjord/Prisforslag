using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.DataAksess;
using Corinor.Presentasjon;
using Corinor.Modell.Heltre;

namespace Corinor.Kontrollbehandling
{
    public class HeltreplateService
    {

        DataAksess2 db;

        public HeltreplateService(DataAksess2 db)
        {
            this.db = db;
        }

        internal string[] getTykkelser()
        {
            if (db == null) return null;

            List<string> liste = new List<string>();

            foreach (HeltreProdukt produkt in db.Produktbeholder.HeltreProduktliste)
                if (!liste.Contains(produkt.Tykkelse))
                    liste.Add(produkt.Tykkelse);

            return liste.ToArray();
        }

        internal string[] getBenkeplateTyper(string tykkelse)
        {
            if (db == null) return null;

            List<string> liste = new List<string>();

            foreach (HeltreProdukt produkt in db.Produktbeholder.HeltreProduktliste)
                if (!liste.Contains(produkt.Type) && produkt.Tykkelse == tykkelse)
                    liste.Add(produkt.Type);

            return liste.ToArray();
        }

        internal string[] getTreslag(string tykkelseValgt, string type)
        {
            List<string> liste = new List<string>();
            foreach (HeltreProdukt produkt in db.Produktbeholder.HeltreProduktliste)
                if (!liste.Contains(produkt.Treslag) && produkt.Tykkelse == tykkelseValgt && produkt.Type == type)
                    liste.Add(produkt.Treslag);

            return liste.ToArray();
        }

 
        internal string[] getStørrelser(string tykkelse, string type, string treslag)
        {
            List<string> liste = new List<string>();
            foreach (HeltreProdukt produkt in db.Produktbeholder.HeltreProduktliste)
                if (!liste.Contains(produkt.DybdeintervallStørrelse) && produkt.Tykkelse == tykkelse && produkt.Type == type && produkt.Treslag == treslag)
                    liste.Add(produkt.DybdeintervallStørrelse);

            return liste.ToArray();


        }

        internal double getPris(string tykkelse, string treslag, string benkeplateType, string størrelse, int lengdeEllerAntall)
        {
            if (db == null) return 0;
            if (lengdeEllerAntall == 0) return 0;

            HeltreProdukt produkt = null;

            foreach (HeltreProdukt p in db.Produktbeholder.HeltreProduktliste)
                if (p.Tykkelse == tykkelse && p.Treslag == treslag && p.Type == benkeplateType && p.DybdeintervallStørrelse == størrelse)
                {
                    produkt = p;
                    break;
                }


            if (produkt == null) 
                return 0;
            else if (produkt.PrisPer == HeltreProdukt.PerPrisType.perLøpemeter)
                return Math.Round((lengdeEllerAntall / 1000.0) * produkt.Pris, 2);
            else
                return lengdeEllerAntall * produkt.Pris; 
        }

        internal double getPrisPerEnhet(string tykkelse, string treslag, string benkeplateType, string størrelse, int lengdeEllerAntall)
        {
            if (db == null) return 0;
            if (lengdeEllerAntall == 0) return 0;

            HeltreProdukt produkt = null;

            foreach (HeltreProdukt p in db.Produktbeholder.HeltreProduktliste)
                if (p.Tykkelse == tykkelse && p.Treslag == treslag && p.Type == benkeplateType && p.DybdeintervallStørrelse == størrelse)
                {
                    produkt = p;
                    break;
                }


            if (produkt == null)
                return 0;
            else 
                return produkt.Pris;
        }

        internal HeltreProdukt.PerPrisType getBenkeplatePtype(string tykkelse, string treslag, string benkeplateType, string størrelse)
        {
            if (db == null) return  HeltreProdukt.PerPrisType.perAntall;

            foreach (HeltreProdukt p in db.Produktbeholder.HeltreProduktliste)
                if (p.Tykkelse == tykkelse && p.Treslag == treslag && p.Type == benkeplateType && p.DybdeintervallStørrelse == størrelse)
                {
                    return p.PrisPer;
                }

            return  HeltreProdukt.PerPrisType.perAntall;
        }


    }



}
