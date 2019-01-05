using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.DataAksess;
using System.Data;
using Corinor.Presentasjon;
using System.Globalization;
using Corinor.Presentasjon.Forslag;
using Corinor.Modell.Tilvalg;

namespace Corinor.Kontrollbehandling
{
    public class TilvalgHeltreplateService
    {
        ForslagPresentasjon forslag;

        public TilvalgHeltreplateService(ForslagPresentasjon forslag)
        {
            this.forslag = forslag;
        }

        internal KontrollProduktPresentasjon getTilvalg(TilvalgGruppe tilvalgGruppe) //gruppeNavn
        {
            KontrollProduktPresentasjon tilvalg = null;

            if (tilvalgGruppe.tilvalgListe.Count == 1)
                tilvalg = opprettNyttTilvalg(tilvalgGruppe.tilvalgListe[0]);
            else
            {
                KontrollProduktPresentasjon[] tps = new KontrollProduktPresentasjon[tilvalgGruppe.tilvalgListe.Count];
                for (int i = 0; i < tps.Length; i++)
                {
                    tps[i] = opprettNyttTilvalg(tilvalgGruppe.tilvalgListe[i]);
                    //if (tps[i].ProduktNavn.Length + tilvalgGruppe.GruppeTittel.Length < 70)
                    //    tps[i].ProduktNavn = tilvalgGruppe.GruppeTittel + ": " + tps[i].ProduktNavn;
                    //else
                    //{
                    //    string g = "";
                    //    string t = tps[i].ProduktNavn;

                    //    int krymping = tps[i].ProduktNavn.Length + tilvalgGruppe.GruppeTittel.Length - 70;

                    //    if (krymping > 0 && tilvalgGruppe.GruppeTittel.Length > krymping)
                    //        g = tilvalgGruppe.GruppeTittel.Remove(tilvalgGruppe.GruppeTittel.Length - krymping) + "...";

                    //    tps[i].ProduktNavn = g + ": " + t;
                    //}

                    //tps[i].ProduktNavn = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";//40
                    //tps[i].ProduktNavn = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";//50
                    //tps[i].ProduktNavn = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";//70
                }

                tilvalg = new TilvalgGruppePresentasjon(tilvalgGruppe.GruppeTittel, 0, tps);
            }

            return tilvalg;
        }

        public KontrollProduktPresentasjon opprettNyttTilvalg(TilvalgElement tilvalgElement)
        {
            KontrollProduktPresentasjon tilvalg = null;

            switch (tilvalgElement.PrisTypeBeregning)
            { 
                case TilvalgElement.PrisBeregningType.normal:
                    tilvalg = new TilvalgEnkelPris(tilvalgElement.TilvalgTittel, tilvalgElement.Pris);
                    break;
                case TilvalgElement.PrisBeregningType.totalsumXprosent:
                    tilvalg = new TilvalgTotalsumXprosentTilvalg(tilvalgElement.TilvalgTittel, tilvalgElement.Pris, forslag, tilvalgElement.MinstePris);
                    break;
                case TilvalgElement.PrisBeregningType.noramlTusendel:
                    tilvalg = new TilvalgEnkelPrisTusendel(tilvalgElement.TilvalgTittel, tilvalgElement.Pris);
                    break;
                case TilvalgElement.PrisBeregningType.kvadratmeter:
                    tilvalg = new TilvalgKvadratmeterPris(tilvalgElement.TilvalgTittel, tilvalgElement.Pris);
                    break;
                case TilvalgElement.PrisBeregningType.egendefinert:
                    tilvalg = new TilvalgEgendefinertPris(tilvalgElement.TilvalgTittel, tilvalgElement.Pris);
                    break;
            }

            return tilvalg;
        }
    }
}
