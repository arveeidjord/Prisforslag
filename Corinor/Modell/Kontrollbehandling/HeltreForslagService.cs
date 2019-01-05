using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corinor.Presentasjon;
using System.ComponentModel;
using System.Windows.Controls;
using Corinor.Kontroller;
using Corinor.DataAksess;
using System.Windows;
using Corinor.Vinduer;
using System.Collections.ObjectModel;
using Corinor.Presentasjon.Forslag;

namespace Corinor.Kontrollbehandling
{
    public class HeltreForslagService
    {
        readonly DataAksess2 db;

        public HeltreForslagPresentasjon ForslagSomVises { get; private set; }

        public string HuskPrisgruppenavn = "";
        private string HuskTreslag = "";

        public HeltreForslagService(DataAksess2 db)
        {
            this.db = db;
            ForslagSomVises = new HeltreForslagPresentasjon();

             //for (int i = 0; i < 2;i++ )
             //      ForslagSomVises.Add(new HeltreplatePresentasjon("Bjørk", "Benkeplate " + i, "30 mm fingerskjørtet", "Intill 300", 3000, 1400));
        }

        internal void leggTilNyHeltreplate(FrameworkElement kontrollIvindu)
        {
            HeltreplateVindu heltreplateVindu = new HeltreplateVindu(db, HuskTreslag);
            heltreplateVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(kontrollIvindu); ;
            heltreplateVindu.ShowDialog();

            if (heltreplateVindu == null || heltreplateVindu.HeltreplateSomEndres == null || heltreplateVindu.DialogResult != true)
                return;
            
            HuskTreslag = heltreplateVindu.HuskTreslag;

            if (!ForslagSomVises.Produkter.Contains(heltreplateVindu.HeltreplateSomEndres))
                ForslagSomVises.Add(heltreplateVindu.HeltreplateSomEndres);

        }

        internal void leggTilAnnetTilvalg(FrameworkElement kontrollIvindu)
        {
            TilvalgVindu tilvalgVindu = new TilvalgVindu(db, ForslagSomVises, Modell.Produkt.DelingType.Heltre);
            tilvalgVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(kontrollIvindu);
            tilvalgVindu.ShowDialog();


            if (tilvalgVindu == null || tilvalgVindu.TilvalgSomEndres == null || tilvalgVindu.DialogResult != true)
                return;

            if (!ForslagSomVises.Tilvalg.Contains(tilvalgVindu.TilvalgSomEndres))
                ForslagSomVises.AddTilvalg(tilvalgVindu.TilvalgSomEndres);

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


        internal void leggTilKummer(Kontroller.Forslag.HeltreForslagKontroll heltreForslagKontroll)
        {
            CorianplateVindu3 corianVindu = new CorianplateVindu3(db, Modell.Produkt.DelingType.HeltreKum, Modell.Produkt.DelingType.CorianKumHeltreKum, true, HuskPrisgruppenavn);
            corianVindu.Title = "Legg til kummer";
            corianVindu.Owner = Hjelpeklasser.VisualTree.getWindowFromParent(heltreForslagKontroll);
            corianVindu.ShowDialog();

            HuskPrisgruppenavn = corianVindu.HuskFarge;

            if (corianVindu == null || corianVindu.Produkt == null || corianVindu.DialogResult != true)
                return;

            if (!ForslagSomVises.Produkter.Contains(corianVindu.Produkt))
                ForslagSomVises.AddTilvalg(corianVindu.Produkt);
        }
    }
}
