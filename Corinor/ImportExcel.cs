using ClosedXML.Excel;
using Corinor.DataAksess;
using Corinor.Modell.CorianProdukt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Corinor
{
    public class ImportExcel
    {
        static string pinusRadiata = "Pinus Radiata";
        static string pinusRadiataStavbredde90 = "Pinus Radiata, Stavbredde 90 mm";
        static string eikRustik = "Eik rustik børstet";
        static string EikRustikStavbredde75 = "Eik rustik børstet, Stavbredde 79 mm";

        static int RadOverFlereLinjer = 372;
        public static void Start(ProduktBeholder p)
        {
            foreach (var pp in p.HeltreProduktliste)
            {
                //pp.DybdeintervallStørrelse = pp.DybdeintervallStørrelse.Replace("Inntill", "inntil");
                //pp.DybdeintervallStørrelse = pp.DybdeintervallStørrelse.Replace("Inntil", "inntil");

                pp.Pris = -1;

                //if (pp.DybdeintervallStørrelse.Trim() == "450-699")
                //    pp.DybdeintervallStørrelse = "451-700";

                //if (pp.DybdeintervallStørrelse.Trim() == "0-299")
                //    pp.DybdeintervallStørrelse = "0-300";

                //if (pp.DybdeintervallStørrelse.Trim() == "300-449")
                //    pp.DybdeintervallStørrelse = "301-450";

                //if (pp.DybdeintervallStørrelse.Trim() == "700-930")
                //    pp.DybdeintervallStørrelse = "701-930";

                //if (pp.DybdeintervallStørrelse.Trim() == "931-1250")
                //    pp.DybdeintervallStørrelse = "961-1250";

                //pp.Treslag = pp.Treslag.Replace("Amerikansk", "amerikansk");
                //pp.Treslag = pp.Treslag.Replace("Europeisk", "europeisk");
                //if (pp.Treslag.Contains(pinusRadiata)) pp.Treslag = pinusRadiataStavbredde90;
                //if (pp.Treslag.Contains(eikRustik)) pp.Treslag = EikRustikStavbredde75;
                //if (pp.Treslag.Contains("Bambus design vertikal T. 38")) pp.Treslag = "Bambus design vertikal T. 38";

                //pp.Treslag = pp.Treslag.Replace("Sort eik", "Eik sort");
                //pp.Treslag = pp.Treslag.Replace("Bambus vertikal", "Bambus vertikal herning");
            }


            var mmInt = 0;
            var mmTekst = new[] { " fingerskjøtet", " fingerskjøtet", " hele staver", " hele staver" };
            //var kolonner = new[] { "S", "X", "AB", "AG", "AL", "AR", "AV", "AZ", "BD", "BH", "BL" };
            //var kolonner = new[] { "M", "O", "S", "X", "AB", "AF", "AL", "AR", "AV", "AZ", "BD" };
            var kolonner = new[] { "J", "L", "O", "T", "X", "AA", "AG", "AM", "AP", "AS", "AW" };


            var stoerrelser = new[] { "", "", "", "", "", "", "", "", "", "", "" };
            var type = new[] { "Benkeplate", "Benkeplate", "Benkeplate", "Benkeplate", "Benkeplate", "Halv hjørneplate", "Halv hjørneplate", "Halv hjørneplate", "Hel hjørneplate", "Hel hjørneplate", "Hel hjørneplate" };
            var fil = @"E:\Corinor prosjekt\Corinor 2019\Corinor-prisliste-2019-191218.xlsx";
            var wb = new XLWorkbook(fil);

            var ws = wb.Worksheet("Table 1");
            var mm = "";
            var newLineRader = 0;
            for (int rad = 339; rad < 385; rad++)
            {
                var celleA = ws.Cell("A" + rad).Value.ToString().TrimEnd();

                var tallIStarten = 0;
                if (celleA.Length >= 5 && celleA.Substring(0, 5).EndsWith(" mm") && int.TryParse(celleA.Substring(0, 2), out tallIStarten))
                {
                    celleA = celleA.Substring(0, 5);
                    mm = celleA + mmTekst[mmInt];
                    mmInt++;
                    for (int i = 0; i < 11; i++)
                    {
                        //var space = i == 0 ? " " : "";
                        var adresse = kolonner[i] + (rad + 1);

                        if (adresse == "AW371")
                            stoerrelser[i] = "1200x1200";
                        else
                            stoerrelser[i] = ws.Cell(adresse).Value.ToString().Trim().Replace(Environment.NewLine, "");
                        //stoerrelser[i] += space + ws.Cell(kolonner[i] + (rad + 2)).Value.ToString().Trim();
                    }
                    
                    rad++;
                    continue;
                }

                if (rad == RadOverFlereLinjer)
                {
                    celleA = celleA.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)[newLineRader].Trim();
                }


                //if (celleA == pinusRadiata)
                //    celleA = pinusRadiataStavbredde90;

                //if (celleA == eikRustik)
                //    celleA = EikRustikStavbredde75;

                if (celleA == "Eik rustik" && mm == "40 mm fingerskjøtet")
                    celleA = "Eik rustik, Stavbredde 79 mm";

                //if(celleA == "Eik sort")
                //    celleA = "Sort eik";

                //var prods = p.HeltreProduktliste
                //    .Where(x => x.Tykkelse == mm)
                //    .Where(x => x.Treslag == celleA)
                //    ;

                for (int i = 0; i < 11; i++)
                {
                    var s = stoerrelser[i];
                    var k = kolonner[i];
                    var t = type[i];

                    if (string.IsNullOrEmpty(s))
                        MessageBox.Show("FEIL: Størrelse har ikke noe verdi. Sjekk om har satt opp riktig kolonner i kolonner array");

                    var ppListe = p.HeltreProduktliste
                    .Where(x => x.Tykkelse == mm)
                    .Where(x => x.Treslag == celleA)
                    .Where(x => x.Type == t)
                    .Where(x => x.DybdeintervallStørrelse == s)
                    .ToList()
                    ;
                    double? nypris = -1;

                    if (rad == RadOverFlereLinjer)
                    {
                        double tmp = -1;
                        double.TryParse((ws.Cell(k + rad).Value as string).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)[newLineRader], out tmp);
                        if (tmp != -1)
                            nypris = tmp;

                    }
                    else
                    {
                        var vv = Convert.ToString(ws.Cell(k + rad).Value);
                        var nyPrisString = vv as string;
                        nyPrisString = nyPrisString.Replace(" ", "");
                        if (!string.IsNullOrEmpty(nyPrisString))
                            nypris = Convert.ToDouble(nyPrisString); // as double?;
                    }

                    if (nypris.HasValue)
                    {
                        if (ppListe.Count() != 1)
                        {
                            MessageBox.Show("Feil antall");
                        }

                        var pp = ppListe.First();
                        if (pp.Pris != -1)
                            MessageBox.Show("Produktet har allerede pris");

                        pp.Pris = nypris.Value;
                    }
                }

                if (rad == RadOverFlereLinjer && newLineRader < 8)
                {
                    newLineRader++;
                    rad--;
                }

                //if (prods.Count() == 11)
                //{

                //}
                //else
                //{
                //    MessageBox.Show("Feil antall");
                //}

            }

            var f = p.HeltreProduktliste.Where(x => x.Pris == -1).ToList();
            MessageBox.Show("Feil på produkter: " + f.Count);
            wb.Dispose();
        }

        public static void StartCorian2(ProduktBeholder produktBeholder)
        {
            var antallPriserNullstilt = 0;
            foreach (var p in produktBeholder.ProduktListe)
            {

                p.Navn = p.Navn.Replace("intill", "inntil");
                p.Navn = p.Navn.Replace("og utsparing", "og utspring");


                for (int i = 0; i < 5; i++)
                {
                    if (p.Prisgrupper[i].Prisgrunnlag > 0 && p.Prisgrupper[i].Avhengighet == null)
                    {
                        p.Prisgrupper[i].Prisgrunnlag = -999;
                        antallPriserNullstilt++;
                    }
                }
            }



            var mmInt = 0;

            var fil = @"C:\P\Corinor 2018\Corinor-prisliste-20180305 Corian.xlsx";
            var wb = new XLWorkbook(fil);

            var antallForMangeFunnet = 0;
            var antallPriserPåProdukterSatt = 0;
            var antallProduktPrisgrupperHvorPrisAlleredeErSatt = 0;




            for (int sheetNr = 4; sheetNr < 11; sheetNr++) //XLS SHEET
            {
                var ws = wb.Worksheet("Sheet" + sheetNr);

                var antallRaderFunnet = 0;

                for (int rad = 1; rad < 20; rad++)
                {

                    var celleA = ws.Cell("A" + rad).Value.ToString().TrimEnd();

                    if (celleA.ToLower().Contains("Mixa 851"))
                    {

                    }

                    if (string.IsNullOrEmpty(celleA))
                        continue;

                    antallRaderFunnet++;

                    if (celleA.ToLower().StartsWith("a)")
                        || celleA.ToLower().StartsWith("b)")
                        || celleA.ToLower().StartsWith("c)")
                        || celleA.ToLower().StartsWith("d)")
                        || celleA.ToLower().StartsWith("e)")
                        || celleA.ToLower().StartsWith("f)")
                        || celleA.ToLower().StartsWith("g)")
                        || celleA.ToLower().StartsWith("h)")
                        )
                    {

                        IEnumerable<CorianProdukt> produkter = null;

                        if (celleA.ToLower().Contains("rillefelt ved kum"))
                        {
                            produkter = produktBeholder.ProduktListe.Where(x => x.Navn.ToLower().Contains("rillefelt ved kum"));
                        }
                        else
                        {
                            var n = 0;
                            while (produkter == null || (produkter.Count() == 0 && n < celleA.Length))
                            {
                                var celleASub = celleA.Remove(0, n++);
                                produkter = produktBeholder.ProduktListe.Where(x => x.Navn.ToLower().Contains(celleASub.ToLower()));
                            }

                            produkter = produkter.Where(x => x.Prisgruppe.Avhengighet == null);

                            if (produkter.Count() > 1)
                            {
                                var celleB = ws.Cell("B" + rad).Value.ToString().TrimEnd().Replace("d.", "").Replace(" ", "");
                                produkter = produkter.Where(x => x.Navn.ToLower().Replace(" ", "").Contains(celleB.ToLower()));
                            }

                            if (produkter.Count() > 1)
                            {
                                if (celleA.ToLower().Contains("andre tykkelser"))
                                    produkter = produkter.Where(x => x.Navn.ToLower().Contains("andre tykkelser"));
                                else
                                    produkter = produkter.Where(x => !x.Navn.ToLower().Contains("andre tykkelser"));

                                //if (celleA.ToLower().Contains("endevange"))
                                //    produkter = produkter.Where(x => x.Navn.ToLower().Contains("endevange"));
                                //else
                                //    produkter = produkter.Where(x => !x.Navn.ToLower().Contains("endevange"));
                            }
                        }

                        

                        if (produkter.Count() == 1 || celleA.ToLower().Contains("rillefelt ved kum"))
                        {
                            foreach (var produkt in produkter)
                            {
                                var priser = new int[5];
                                char col = 'A';

                                int i = 0;
                                while (i < 5 && i < 100)
                                {
                                    int pris = 0;
                                    int.TryParse(ws.Cell(col.ToString() + rad).Value.ToString().TrimEnd(), out pris);

                                    col++;
                                    if (pris > 0)
                                    {
                                        priser[i] = pris;
                                        i++;
                                    }
                                }


                                for (int k = 0; k < 5; k++)
                                {
                                    if (produkt.Prisgrupper[k].Prisgrunnlag == -999)
                                    {
                                        if (priser[k] > 100)//Ingen priser under 100
                                            produkt.Prisgrupper[k].Prisgrunnlag = priser[k];
                                        else
                                            MessageBox.Show("FEIL: Pris er 0");
                                    }
                                    else
                                    {
                                        antallProduktPrisgrupperHvorPrisAlleredeErSatt++;
                                        //MessageBox.Show("FEIL: Finnes ikke pris på produktet fra før!");
                                    }
                                }

                                antallPriserPåProdukterSatt++;
                            }
                        }
                        else if (produkter.Count() > 1)
                        {
                            antallForMangeFunnet++;
                        }



                    }



                }
            }

            var f = produktBeholder.ProduktListe.Where(x => x.Prisgrupper[0].Prisgrunnlag == -999
                                                || x.Prisgrupper[1].Prisgrunnlag == -999
                                                || x.Prisgrupper[2].Prisgrunnlag == -999
                                                || x.Prisgrupper[3].Prisgrunnlag == -999
                                                || x.Prisgrupper[4].Prisgrunnlag == -999).ToList();
            MessageBox.Show("Feil på produkter: " + f.Count);
            wb.Dispose();
        }

        public static void StartCorian3(ProduktBeholder produktBeholder)
        {
            var antallPriserNullstilt = 0;
            //foreach (var p in produktBeholder.ProduktListe)
            //{

            //    p.Navn = p.Navn.Replace("intill", "inntil");
            //    p.Navn = p.Navn.Replace("og utsparing", "og utspring");


            //    for (int i = 0; i < 5; i++)
            //    {
            //        if (p.Prisgrupper[i].Prisgrunnlag > 0 && p.Prisgrupper[i].Avhengighet == null)
            //        {
            //            p.Prisgrupper[i].Prisgrunnlag = -999;
            //            antallPriserNullstilt++;
            //        }
            //    }
            //}

            var fil = @"C:\P\Corinor 2018\Corinor-prisliste-20180305 Corian.xlsx";
            var wb = new XLWorkbook(fil);

            var antallForMangeFunnet = 0;
            var antallPriserPåProdukterSatt = 0;
            var antallProduktPrisgrupperHvorPrisAlleredeErSatt = 0;




            for (int sheetNr = 34; sheetNr < 37; sheetNr++) //XLS SHEET
            {
                var ws = wb.Worksheet("Sheet" + sheetNr);

                var antallRaderFunnet = 0;

                for (int rad = 1; rad < 20; rad++)
                {

                    var celleA = ws.Cell("A" + rad).Value.ToString().TrimEnd();

                    if (celleA.ToLower().Contains("Mixa 851"))
                    {

                    }

                    if (string.IsNullOrEmpty(celleA))
                        continue;

                    antallRaderFunnet++;

                    if (celleA.ToLower().StartsWith("a)")
                        || celleA.ToLower().StartsWith("b)")
                        || celleA.ToLower().StartsWith("c)")
                        || celleA.ToLower().StartsWith("d)")
                        || celleA.ToLower().StartsWith("e)")
                        || celleA.ToLower().StartsWith("f)")
                        || celleA.ToLower().StartsWith("g)")
                        || celleA.ToLower().StartsWith("h)")
                        || celleA.ToLower().StartsWith("i)")
                        || celleA.ToLower().StartsWith("j)")
                        || celleA.ToLower().StartsWith("k)")
                        || celleA.ToLower().StartsWith("l)")
                        || celleA.ToLower().StartsWith("m)")
                        )
                    {

                        IEnumerable<CorianProdukt> produkter = null;

                        if (celleA == "f)")
                            celleA = ws.Cell("B" + rad).Value.ToString().TrimEnd();

                        if (celleA.ToLower().Contains("rillefelt ved kum"))
                        {
                            produkter = produktBeholder.ProduktListe.Where(x => x.Navn.ToLower().Contains("rillefelt ved kum"));
                        }
                        else
                        {
                            var n = 0;
                            while (produkter == null || (produkter.Count() == 0 && n < celleA.Length))
                            {
                                var celleASub = celleA.Remove(0, n++);
                                produkter = produktBeholder.ProduktListe.Where(x => x.Navn.ToLower().Contains(celleASub.ToLower()));
                            }

                            produkter = produkter.Where(x => x.Prisgruppe.Avhengighet == null);

                            if (produkter.Count() > 1)
                            {
                                var celleB = ws.Cell("B" + rad).Value.ToString().TrimEnd().Replace("d.", "").Replace(" ", "");
                                produkter = produkter.Where(x => x.Navn.ToLower().Replace(" ", "").Contains(celleB.ToLower()));
                            }

                            if (produkter.Count() > 1)
                            {
                                var celleB = ws.Cell("C" + rad).Value.ToString().TrimEnd().Replace("d.", "").Replace(" ", "");
                                produkter = produkter.Where(x => x.Navn.ToLower().Replace(" ", "").Contains(celleB.ToLower()));
                            }

                            if (produkter.Count() > 1)
                            {
                                if (celleA.ToLower().Contains("andre tykkelser"))
                                    produkter = produkter.Where(x => x.Navn.ToLower().Contains("andre tykkelser"));
                                else
                                    produkter = produkter.Where(x => !x.Navn.ToLower().Contains("andre tykkelser"));

                                //if (celleA.ToLower().Contains("endevange"))
                                //    produkter = produkter.Where(x => x.Navn.ToLower().Contains("endevange"));
                                //else
                                //    produkter = produkter.Where(x => !x.Navn.ToLower().Contains("endevange"));
                            }
                        }



                        if (produkter.Count() == 1 || celleA.ToLower().Contains("rillefelt ved kum"))
                        {
                            foreach (var produkt in produkter)
                            {
                                var priser = new int[5];
                                char col = 'A';

                                int i = 0;
                                while (i < 5 && i < 100)
                                {
                                    int pris = 0;
                                    int.TryParse(ws.Cell(col.ToString() + rad).Value.ToString().TrimEnd(), out pris);

                                    col++;
                                    if (pris > 0)
                                    {
                                        priser[i] = pris;
                                        i++;
                                    }
                                }


                                for (int k = 0; k < 5; k++)
                                {
                                    if (produkt.Prisgrupper[k].Prisgrunnlag == -999)
                                    {
                                        if (priser[k] > 100)//Ingen priser under 100
                                            produkt.Prisgrupper[k].Prisgrunnlag = priser[k];
                                        else
                                            MessageBox.Show("FEIL: Pris er 0");
                                    }
                                    else
                                    {
                                        antallProduktPrisgrupperHvorPrisAlleredeErSatt++;
                                        //MessageBox.Show("FEIL: Finnes ikke pris på produktet fra før!");
                                    }
                                }

                                antallPriserPåProdukterSatt++;
                            }
                        }
                        else if (produkter.Count() > 1)
                        {
                            antallForMangeFunnet++;
                        }



                    }



                }
            }

            var f = produktBeholder.ProduktListe.Where(x => x.Prisgrupper[0].Prisgrunnlag == -999
                                                || x.Prisgrupper[1].Prisgrunnlag == -999
                                                || x.Prisgrupper[2].Prisgrunnlag == -999
                                                || x.Prisgrupper[3].Prisgrunnlag == -999
                                                || x.Prisgrupper[4].Prisgrunnlag == -999).ToList();
            MessageBox.Show("Feil på produkter: " + f.Count);
            wb.Dispose();
        }

        internal static void StartCorian(ProduktBeholder produktBeholder)
        {
            //foreach (var p in produktBeholder.ProduktListe)
            //{
            //    if (p.Prisgrupper[0].Prisgrunnlag > 0) p.Prisgrupper[0].Prisgrunnlag = -999;
            //    if (p.Prisgrupper[1].Prisgrunnlag > 0) p.Prisgrupper[1].Prisgrunnlag = -999;
            //    if (p.Prisgrupper[2].Prisgrunnlag > 0) p.Prisgrupper[2].Prisgrunnlag = -999;
            //    if (p.Prisgrupper[3].Prisgrunnlag > 0) p.Prisgrupper[3].Prisgrunnlag = -999;
            //    if (p.Prisgrupper[4].Prisgrunnlag > 0) p.Prisgrupper[4].Prisgrunnlag = -999;
                
            //}


            var fil = @"C:\P\Corinor 2018\Corinor-prisliste-20180305.txt";
            var linjer = File.ReadAllLines(fil, Encoding.GetEncoding(1252));
            var antallProdukterFaaNyPris = 0;
            for (int i = 0; i < linjer.Length; i++)
            {
                var l = linjer[i];

                if (l.Contains(") Pr."))
                {
                    var spl = l.Split(new []{ " " },StringSplitOptions.RemoveEmptyEntries);
                    var priser = new double[] { 
                        double.Parse(spl[spl.Length - 5]), 
                        double.Parse(spl[spl.Length - 4]), 
                        double.Parse(spl[spl.Length - 3]), 
                        double.Parse(spl[spl.Length - 2]), 
                        double.Parse(spl[spl.Length - 1]) 
                    };

                    if (!(spl[spl.Length - 5].Length == spl[spl.Length - 1].Length
                        && spl[spl.Length - 5].Length == spl[spl.Length - 2].Length
                        && spl[spl.Length - 5].Length == spl[spl.Length - 3].Length
                        && spl[spl.Length - 5].Length == spl[spl.Length - 4].Length))
                    {
                        //MessageBox.Show("Priser ikke riktig lengde");
                        //return;
                    }

                    //var ww = produktBeholder.ProduktListe.FirstOrDefault().Navn;
                    //var w222 = ww.ToLower().Substring(0, 8).Replace(" ", "");
                    //var l2 = l.ToLower().Replace(" ", "");
                    //var fff = l2.Contains(w222);

                    //var kk = "450-699".Contains("450-699");

                    int n = 8;
                    var w = produktBeholder.ProduktListe
                        .Where(x => l.ToLower().Contains(x.ProduktKategori.ToLower().Substring(0, 6)))
                        .Where(x => l.ToLower().Replace(" ", "").Contains(x.Navn.ToLower().Substring(0, n).Replace(" ", "")))
                        .Where(x => x.Prisgruppe.Avhengighet == null)
                        .ToList();

                    if (w.Count > 1)
                    {
                        while (w.Count > 1)
                        {
                            n++;
                            w = produktBeholder.ProduktListe
                                .Where(x => l.ToLower().Contains(x.ProduktKategori.ToLower().Substring(0, 6)))
                                .Where(x => l.ToLower().Replace(" ", "").Contains(x.Navn.ToLower().Substring(0, n).Replace(" ", "")))
                                .Where(x => x.Prisgruppe.Avhengighet == null)
                                .ToList();
                        }
                        
                    }

                    if (w.Count > 1)
                    {
                    }

                    if (w.Count() == 0)
                    {
                        
                    }

                    if (w.Count == 1)
                    {
                        var produkt = w.FirstOrDefault();
                        produkt.Prisgrupper[0].Prisgrunnlag = priser[0];
                        produkt.Prisgrupper[1].Prisgrunnlag = priser[1];
                        produkt.Prisgrupper[2].Prisgrunnlag = priser[2];
                        produkt.Prisgrupper[3].Prisgrunnlag = priser[3];
                        produkt.Prisgrupper[4].Prisgrunnlag = priser[4];

                        antallProdukterFaaNyPris++;
                    }
                        
                        //foreach (var produkt in produktBeholder.ProduktListe)
                    //{
                        

                    //    if (l.ToLower().Contains(produkt.ProduktKategori.ToLower().Substring(0, 6)))
                    //        //&& produkt.Prisgrupper[0].Prisgrunnlag == -999)
                    //    {
                    //        produkt.Prisgrupper[0].Prisgrunnlag = priser[0];
                    //        produkt.Prisgrupper[1].Prisgrunnlag = priser[1];
                    //        produkt.Prisgrupper[2].Prisgrunnlag = priser[2];
                    //        produkt.Prisgrupper[3].Prisgrunnlag = priser[3];
                    //        produkt.Prisgrupper[4].Prisgrunnlag = priser[4];

                    //        break;
                    //    }
                    //}

                }

            }

            MessageBox.Show(antallProdukterFaaNyPris + " av " + produktBeholder.ProduktListe.Count +  " produkter har fått ny pris");

        }


        public static void StartCorian4(ProduktBeholder produktBeholder)
        {
            var antallPriserNullstilt = 0;
            foreach (var p in produktBeholder.ProduktListe)
            {

                //p.Navn = p.Navn.Replace("intill", "inntil");
                //p.Navn = p.Navn.Replace("og utsparing", "og utspring");

                if (p.Navn.Contains("450-699"))
                    p.Navn = p.Navn.Replace("450-699", "451-700");

                if (p.Navn.Contains("0-299"))
                    p.Navn = p.Navn.Replace("0-299", "0-300");

                if (p.Navn.Contains("300-449"))
                    p.Navn = p.Navn.Replace("300-449", "301-450");

                if (p.Navn.Contains("700-930"))
                    p.Navn = p.Navn.Replace("700-930", "701-930");


                if (p.Navn.Contains("0-349"))
                    p.Navn = p.Navn.Replace("0-349", "0-350");

                if (p.Navn.Contains("350-699"))
                    p.Navn = p.Navn.Replace("350-699", "351-700");

                if (p.Navn.Contains("700-929"))
                    p.Navn = p.Navn.Replace("700-929", "701-930");

                if (p.Navn.Contains("930-1230"))
                    p.Navn = p.Navn.Replace("930-1230", "931-1250");

                //if (pp.DybdeintervallStørrelse.Trim() == "0-299")
                //    pp.DybdeintervallStørrelse = "0-300";

                //if (pp.DybdeintervallStørrelse.Trim() == "300-449")
                //    pp.DybdeintervallStørrelse = "301-450";

                //if (pp.DybdeintervallStørrelse.Trim() == "700-930")
                //    pp.DybdeintervallStørrelse = "701-930";


                for (int i = 0; i < 5; i++)
                {
                    if (p.Prisgrupper[i].Prisgrunnlag > 0 && p.Prisgrupper[i].Avhengighet == null)
                    {
                        p.Prisgrupper[i].Prisgrunnlag = -999;
                        antallPriserNullstilt++;
                    }
                }

                for (int i = 10; i < 12; i++)
                {
                    if (p.Prisgrupper[i].Prisgrunnlag > 0 && p.Prisgrupper[i].Avhengighet == null)
                    {
                        p.Prisgrupper[i].Prisgrunnlag = -999;
                        antallPriserNullstilt++;
                    }
                }
            }




            //var antallForMangeFunnet = 0;
            //var antallPriserPåProdukterSatt = 0;
            //var antallProduktPrisgrupperHvorPrisAlleredeErSatt = 0;


            var fil = @"E:\Corinor prosjekt\Corinor 2019\Corinor-prisliste-2019-191218.xlsx";
            var wb = new XLWorkbook(fil);

            var ws = wb.Worksheet("Table 1");

            var celleBE = "";

            for (int rad = 87; rad < 147; rad++)
            {
                var tmpCelleBE = ws.Cell("BE" + rad).Value.ToString().TrimEnd();
                if (!string.IsNullOrWhiteSpace(tmpCelleBE))
                {
                    celleBE = tmpCelleBE;
                }
                else
                {
                    var celleA = ws.Cell("A" + rad).Value.ToString().TrimEnd();

                    //if (rad >= 103 && rad <= 107)
                    //{
                    //    celleA += " Synlig underside plater og innsider vanger";
                    //}

                    var navn = celleA.ToLower()
                        .Replace("a)", "")
                        .Replace("b)", "")
                        .Replace("c)", "")
                        .Replace("d)", "")
                        .Replace("e)", "")
                        .Replace("f)", "")
                        .Replace("g)", "")
                        .Replace("pr.", "")
                        .Replace(" mm", "mm") //NB
                        .Replace("cm", "") //NB
                        .Replace("lm ", "")
                        .Replace("lm. ", "")
                        .Replace(" stk", "")

                        .Replace("d. ", "")
                        .Trim();

                    if (navn.Contains("tillegg for andre") 
                        || celleA == "pr lm benkeplate"
                        || celleA == "pr. lm plate"
                        || celleA.Contains("Standard radius 12 mm"))
                        continue;

                    var spl = navn.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    var valgtProdukt = produktBeholder.ProduktListe
                        .Where(x => x.ProduktKategori.ToLower() == celleBE.ToLower())
                        ;

                    var prodKat = valgtProdukt.ToList();
                    if (prodKat.Count == 0)
                    {

                    }

                    if (celleBE != "Stålstenger")
                    {
                        foreach (var s in spl)
                        {
                            if (s == "pr")
                                continue;

                            if (s == "per")
                                continue;

                            if (s == "lm")
                                continue;

                            valgtProdukt = valgtProdukt.Where(x => x.Navn.ToLower().Replace(" mm", "mm").Contains(s.Replace(",", "")));

                            if (valgtProdukt.Count() == 0)
                                throw new Exception("0");
                        }
                    }

                    if (spl[0] == "hulkil" && spl[2] == "50mm")
                    {
                        valgtProdukt = valgtProdukt.Where(x => x.Navn.ToLower().Contains(" 50mm"));
                    }

                    //a) Pr. lm benkeplate            d. 451-700mm
                    //451-700 mm: Benkeplate

                    

                    if (valgtProdukt.Count() == 0)
                    {
                        throw new Exception("fant ingen produkter!!!");
                    }

                    var typeA = valgtProdukt.FirstOrDefault(x => x.Navn.ToLower().Contains("type a"));
                    var typeB = valgtProdukt.FirstOrDefault(x => x.Navn.ToLower().Contains("type b"));

                    if (valgtProdukt.Count() == 2 && typeA == null && typeB == null)
                    {
                        var prodMedAvhengighet = valgtProdukt.FirstOrDefault(x => x.Prisgruppe.Avhengighet != null);

                        if (prodMedAvhengighet == null)
                            throw new Exception("Skulle hatt avhengighet når det er 2 produkter");


                        var pris = 0;

                        if (celleBE.ToLower() == "benkeplater")
                            pris = 725;

                        if (celleBE.ToLower() == "Barløsningsplater/Øyplater".ToLower())
                            pris = 1034;

                        if (celleBE.ToLower() == "Hjørneplater".ToLower())
                            pris = 1034;

                        if (pris == 0)
                            throw new Exception("Pris skal ikke være 0!");

                        SetPrisMedAvhengighet(prodMedAvhengighet, 0, pris);
                        SetPrisMedAvhengighet(prodMedAvhengighet, 1, pris);
                        SetPrisMedAvhengighet(prodMedAvhengighet, 2, pris);
                        SetPrisMedAvhengighet(prodMedAvhengighet, 3, pris);
                        SetPrisMedAvhengighet(prodMedAvhengighet, 4, pris);
                        SetPrisMedAvhengighet(prodMedAvhengighet, 10, pris);
                        SetPrisMedAvhengighet(prodMedAvhengighet, 11, pris);
                        
                    }

                    var prodUtenAvhengighet = valgtProdukt.FirstOrDefault(x => x.Prisgruppe.Avhengighet == null);

                    if (typeA != null)
                        prodUtenAvhengighet = typeA;

                    SetPris(prodUtenAvhengighet, rad, ws, "R", 0);
                    SetPris(prodUtenAvhengighet, rad, ws, "W", 1);
                    SetPris(prodUtenAvhengighet, rad, ws, "Z", 2);
                    SetPris(prodUtenAvhengighet, rad, ws, "AH", 3);
                    SetPris(prodUtenAvhengighet, rad, ws, "AN", 4);
                    SetPris(prodUtenAvhengighet, rad, ws, "AQ", 10);
                    SetPris(prodUtenAvhengighet, rad, ws, "AV", 11);

                    if (typeB != null)
                    {
                        SetPris(typeB, rad, ws, "R", 0);
                        SetPris(typeB, rad, ws, "W", 1);
                        SetPris(typeB, rad, ws, "Z", 2);
                        SetPris(typeB, rad, ws, "AH", 3);
                        SetPris(typeB, rad, ws, "AN", 4);
                        SetPris(typeB, rad, ws, "AQ", 10);
                        SetPris(typeB, rad, ws, "AV", 11);
                    }

               


                    //Sett priser
                    //var prodUtenAvhengighet = valgtProdukt.FirstOrDefault(x => x.Prisgruppe.Avhengighet == null);

                    //int pris = Convert.ToInt32(ws.Cell("B" + rad).Value.ToString().TrimEnd());
                    //if (pris <= 0)
                    //    throw new Exception("Feil pris");

                    //prodUtenAvhengighet.Prisgrupper[0].Prisgrunnlag = pris;

                    if (valgtProdukt.Count() > 2)
                    {
                        throw new Exception("Flere enn 2 produkter");

                    }

                }
            }


            var f = produktBeholder.ProduktListe.Where(x => x.Prisgrupper[0].Prisgrunnlag == -999
                                                || x.Prisgrupper[1].Prisgrunnlag == -999
                                                || x.Prisgrupper[2].Prisgrunnlag == -999
                                                || x.Prisgrupper[3].Prisgrunnlag == -999
                                                || x.Prisgrupper[4].Prisgrunnlag == -999
                                                || x.Prisgrupper[10].Prisgrunnlag == -999
                                                || x.Prisgrupper[11].Prisgrunnlag == -999).ToList();
            MessageBox.Show("Feil på produkter: " + f.Count);
            wb.Dispose();
        }

        private static void SetPris(CorianProdukt prodUtenAvhengighet, int rad, IXLWorksheet ws, string xlsColumn, int prisgruppeIndex)
        {
            var cell = ws.Cell(xlsColumn + rad).Value;
            var prisStr = cell.ToString().Replace(" ", "");
            int pris = Convert.ToInt32(prisStr);
            if (pris <= 0)
                throw new Exception("Feil pris");

            prodUtenAvhengighet.Prisgrupper[prisgruppeIndex].Prisgrunnlag = pris;
        }

        private static void SetPrisMedAvhengighet(CorianProdukt prodMedAvhengighet, int prisgruppeIndex, int pris)
        {
            prodMedAvhengighet.Prisgrupper[prisgruppeIndex].Prisgrunnlag = pris;
        }


    }
}
