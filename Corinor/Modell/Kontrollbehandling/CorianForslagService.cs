using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Presentasjon.Forslag;
using Corinor.DataAksess;
using System.Windows;
using Corinor.Vinduer;
using Corinor.Presentasjon;
using System.Windows.Controls;

namespace Corinor.Kontrollbehandling
{
    public class CorianForslagService
    {
        public string HuskFarge = "";  

        public CorianForslagPresentasjon ForslagSomVises { get; private set; }

        readonly DataAksess2 db;

        public CorianForslagService(DataAksess2 db)
        {
            this.db = db;
            ForslagSomVises = new CorianForslagPresentasjon();

            //for (int i = 0; i < 2; i++)
            //    ForslagSomVises.Add(new TilvalgEnkelPris("Benkeplater: 349 - 499 mm", 2400.0, "Prisgruppe 2 (Aqua)"));
        }

        internal void leggTilCorianprodukter(FrameworkElement kontrollIvindu)
        {
            CorianplateVindu3 corianVindu = new CorianplateVindu3(db, Modell.Produkt.DelingType.CorianProdukt, Modell.Produkt.DelingType.CorianProdukt, false, HuskFarge);
            corianVindu.Title = "Legg til Corian-produkter";
            corianVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(kontrollIvindu);
            corianVindu.ShowDialog();

            if (corianVindu == null || corianVindu.Produkt == null || corianVindu.DialogResult != true)
                return;

            HuskFarge = corianVindu.HuskFarge;

            if (!ForslagSomVises.Produkter.Contains(corianVindu.Produkt))
            {
                ForslagSomVises.Add(corianVindu.Produkt);
               
            }
        }

        internal void endreKundeInfo(FrameworkElement kontrollIVindu)
        {
            if (ForslagSomVises == null) return;

            KundeVindu kundeVindu = new KundeVindu(ForslagSomVises);
            kundeVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(kontrollIVindu);
            kundeVindu.ShowDialog();

            if (kundeVindu.DialogResult == true)
            {
                ForslagSomVises.Navn = kundeVindu.navnTekstboks.Text;
                ForslagSomVises.Adresse = kundeVindu.adresseTekstboks.Text;
                ForslagSomVises.Postnummer = kundeVindu.postnummerTekstboks.Text;
                ForslagSomVises.Poststed = kundeVindu.poststedTekstboks.Text;
                ForslagSomVises.Epost = kundeVindu.epostTekstboks.Text;
                ForslagSomVises.Telefonnummere = kundeVindu.telefonnummereTekstboks.Text;
                ForslagSomVises.Kommentar = kundeVindu.kommentarTekstboks.Text;
            }
        }

        internal void leggTilAnnetTilvalg(Kontroller.Forslag.CorianForslagKontroll corianForslagKontroll)
        {
            TilvalgVindu tilvalgVindu = new TilvalgVindu(db, ForslagSomVises, Modell.Produkt.DelingType.Corian);
            tilvalgVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(corianForslagKontroll);
            tilvalgVindu.ShowDialog();


            if (tilvalgVindu == null || tilvalgVindu.TilvalgSomEndres == null || tilvalgVindu.DialogResult != true)
                return;

            if (!ForslagSomVises.Produkter.Contains(tilvalgVindu.TilvalgSomEndres))
            {
                ForslagSomVises.Add(tilvalgVindu.TilvalgSomEndres);
            }
        }

        internal void leggTilKummer(Kontroller.Forslag.CorianForslagKontroll corianForslagKontroll)
        {
            CorianplateVindu3 corianVindu = new CorianplateVindu3(db, Modell.Produkt.DelingType.CorianKum, Modell.Produkt.DelingType.CorianKumHeltreKum, false, HuskFarge);
            corianVindu.Title = "Legg til kummer";
            corianVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(corianForslagKontroll);
            corianVindu.ShowDialog();


            if (corianVindu == null || corianVindu.Produkt == null || corianVindu.DialogResult != true)
                return;

            if (!ForslagSomVises.Produkter.Contains(corianVindu.Produkt))
            {
                ForslagSomVises.Add(corianVindu.Produkt);
            }
        }
    }
}
