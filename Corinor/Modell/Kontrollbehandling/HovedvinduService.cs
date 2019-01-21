using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;
using Corinor.DataAksess;
using System.Windows.Controls;
using Corinor.Kontroller.Forslag;
using Corinor.Presentasjon.Forslag;
using System.Windows.Media;
using Corinor.Kontroller;
using Corinor.Vinduer.EndrePrislister;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Net;
using System.Windows.Media.Imaging;
using Corinor.Properties;
using Corinor.Modell.CorianProdukt;

namespace Corinor.Kontrollbehandling
{
    public class HovedvinduService
    {
        DataAksess2 data;
        Window hovedVindu;
        Image updateImage;
        string updateUrl = @"http://corinor.no/prisprogram/"; //@"http://prisforslag.no/prisliste.data";//@"d:\prisliste.data"; //@"http://prisforslag.no/prisliste.data";

        public HovedvinduService(Window vindu, Image updateImage)
        {
            this.hovedVindu = vindu;
            this.updateImage = updateImage;

            if (!Directory.Exists(Hjelpeklasser.GlobaleUrier.standardMappe2()))
                Directory.CreateDirectory(Hjelpeklasser.GlobaleUrier.standardMappe2());

            //string standardDataFil = "Prisliste.data";
            
            //if (!File.Exists(Hjelpeklasser.GlobaleUrier.prislistefilUri()) && File.Exists(standardDataFil)) 
            //{
            //    try {
            //        File.Copy(standardDataFil, Hjelpeklasser.GlobaleUrier.prislistefilUri(), true);
            //    }
            //    catch (Exception e){
            //        MessageBox.Show("Klarte ikke å kopiere Prisliste.data til lokalt område: \n" + e.Message, "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}

            var res = loadPrisliste(vindu);


            if (!res || Properties.Settings.Default.AutoUpdate)
            {
                //Corinor.Properties.Settings.Default.Reset();
                var sistUpdate = Corinor.Properties.Settings.Default.SisteUpdate; //.AddDays(-7);
                var d = sistUpdate.Date.Subtract(DateTime.Today).Days + 7;
                if (d < 0) d = 0;
                if (d > 7) d = 7;

                var antDagerTilSjekk = d;

                if (!res || (sistUpdate == DateTime.MinValue || antDagerTilSjekk <= 0))
                    OppdaterPrisliste();
                else
                    setUploadImage(string.Format("Forrige sjekk: {0}\n{1} dager til neste sjekk\n(Klikk for å se etter ny prisliste nå)", sistUpdate, antDagerTilSjekk), BildeEnum.OK);
            }
            else
                setUploadImage("Automatisk oppdatering av prisliste er slått av.", Corinor.Kontrollbehandling.HovedvinduService.BildeEnum.autoupdateOff);

            updateImage.IsEnabled = Properties.Settings.Default.AutoUpdate;
        }

        public enum BildeEnum
        {
            Updating,
            OK,
            Error,
            autoupdateOff,
        }

        public void setUploadImage(string tooltip, BildeEnum bilde)
        {
            var header = new TextBlock();
            header.Text = "Prislisteoppdatering";
            header.FontWeight = FontWeights.Bold;

            var tekst = new TextBlock();
            tekst.Text = tooltip;

            var sp = new StackPanel();
            sp.Children.Add(header);
            sp.Children.Add(tekst);

            updateImage.ToolTip = sp;

            var url = "/Corinor;component/Images/gem_okay.png";

            if (bilde == BildeEnum.Updating)
                url = "/Corinor;component/Images/refresh.png";
            else if (bilde == BildeEnum.Error)
                url = "/Corinor;component/Images/gem_cancel_1.png";
            else if (bilde == BildeEnum.autoupdateOff)
                url = "/Corinor;component/Images/information.png";

            var bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(url, UriKind.Relative);
            bi3.EndInit();
            updateImage.Stretch = Stretch.Fill;
            updateImage.Source = bi3;
        }

        public void OppdaterPrisliste()
        {
            setUploadImage("Oppdaterer prisliste...", BildeEnum.Updating);

            var p = new UpdatePrisliste2();
            p.PrislisteDownloded += new EventHandler(p_PrislisteDownloded);
            p.PrislisteDownloadError += new EventHandler(p_PrislisteDownloadError);
            p.GetPrislisteAsync(updateUrl);
        }

        void p_PrislisteDownloadError(object sender, EventArgs e)
        {
            var exception = sender as Exception;

            if (exception == null)
            {
                setUploadImage("Det oppstod en ukjent feil ved oppdatering av prisliste", BildeEnum.Error);
                return;
            }

            if (exception is WebException)
            {
                var webEx = exception as WebException;
                HttpWebResponse errorResponse = webEx.Response as HttpWebResponse;

                if (errorResponse == null)
                {
                    setUploadImage("Prøvde å se etter ny prisliste, men fikk ikke kontakt med server.\nSjekk om du er koblet til Internett.\n(" + exception.Message + ")", BildeEnum.Error);
                
                }
                else if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    setUploadImage("Fant ikke prisliste på server:\n" + exception.Message, BildeEnum.Error);
                }
            }
            else
            {
                setUploadImage("Det oppstod en ukjent feil ved oppdatering av prisliste:\n" + exception.Message, BildeEnum.Error);
            }
        }

        void p_PrislisteDownloded(object sender, EventArgs e)
        {
            var tempfilUrl = sender as string;

            if (tempfilUrl == null)
            {
                setUploadImage("Ingen prisliste lastet ned.\nPrøv igjen senere.", BildeEnum.Error);
                return;
            }

            //#Sjekk om tempfil kan åpnes
            var pbtest = ProduktBeholder.loadData(tempfilUrl);

            if (pbtest == null)
            {
                setUploadImage("Nedlastet prisliste var korrupt og kunne ikke benyttes.\nPrøv igjen senere.", BildeEnum.Error);
                return;
            }
            //###

            //Sjekk om nedlastet prisliste er nyere enn den gjeldene
            if (pbtest.SistEndret > data.Produktbeholder.SistEndret)
            {
                try
                {
                    string til = Hjelpeklasser.GlobaleUrier.prislistefilUri();
                    File.Copy(tempfilUrl, til, true);
                }
                catch
                {
                    setUploadImage("Klarte ikke å overskrive gjeldene prisliste", BildeEnum.Error);
                    return;
                }

                try
                {
                    File.Delete(tempfilUrl);
                }
                catch 
                {
                    Console.WriteLine("Klarte ikke å slette tempfil: " + tempfilUrl);
                }

                loadPrisliste(hovedVindu);

                Corinor.Properties.Settings.Default.SisteUpdate = DateTime.Now;
                Corinor.Properties.Settings.Default.Save();

                var tekst = string.Format("Prislisten er oppdatert.\nForrige sjekk: {0}", Corinor.Properties.Settings.Default.SisteUpdate);
                setUploadImage(tekst, BildeEnum.OK);

                MessageBox.Show("Prislisten er oppdatert", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Corinor.Properties.Settings.Default.SisteUpdate = DateTime.Now;
                Corinor.Properties.Settings.Default.Save();

                var tekst = string.Format("Nyeste prisliste er allerede i systemet.\nForrige sjekk: {0}\n7 dager til neste sjekk", Corinor.Properties.Settings.Default.SisteUpdate);
                setUploadImage(tekst, BildeEnum.OK);
            }
        }



        internal void EndreCorianPriser(MainWindow mainWindow)
        {
            EndreCorianPrisliste endreCorian = new EndreCorianPrisliste(data);
            endreCorian.Owner = mainWindow;
            endreCorian.ShowDialog();

            loadPrisliste(hovedVindu);
            
        }

        internal void EndreHeltrePriser(MainWindow mainWindow)
        {
            EndreHeltrePrisliste endreHeltre = new EndreHeltrePrisliste(data);
            endreHeltre.Owner = mainWindow;
            endreHeltre.ShowDialog();

            loadPrisliste(hovedVindu);
        }

        internal void EndreTilvalg(MainWindow mainWindow)
        {
            EndreTilvalgVindu endreTilvalg = new EndreTilvalgVindu(data);
            endreTilvalg.Owner = mainWindow;
            endreTilvalg.ShowDialog();

            loadPrisliste(hovedVindu);
        }


        #region Settings


        internal bool loadPrisliste(Window window)
        {
            if (data == null) data = new DataAksess2();

            data.Produktbeholder = ProduktBeholder.loadData(Hjelpeklasser.GlobaleUrier.prislistefilUri());
            if (data.Produktbeholder == null)
            {
                data.Produktbeholder = new ProduktBeholder();
                window.Title = "Corinor prisforslag - Prisliste sist endret: -";
                MessageBox.Show("Før programmet kan brukes må prislisten oppdateres.\nVær sikker på at du er koblet til Internett.", "Corinor Prisforslag", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
                window.Title = "Corinor prisforslag - Prisliste sist endret: " + data.Produktbeholder.SistEndret;
            //window.Title = "Corinor prisforslag - Prisliste sist endret: " + data.Produktbeholder.getLastModified(Hjelpeklasser.GlobaleUrier.prislistefilUri());

            //ImportExcel.Start(data.Produktbeholder);
            //ImportExcel.StartCorian2(data.Produktbeholder);
            //ImportExcel.StartCorian4(data.Produktbeholder);

            //var antall = 0;
            //var l = new List<CorianProdukt>();

            //foreach (var p in data.Produktbeholder.ProduktListe)
            //{
            //    var t = false;
            //    foreach (var pg in p.Prisgrupper)
            //    {
            //        if (pg.Prisgrunnlag == 1034)
            //        {
            //            t = true;
            //            //antall++;

            //        }
            //    }

            //    if (t)
            //        l.Add(p);
            //}

            //MessageBox.Show("" + antall);

            return true;
        }

#endregion

        #region Import og Eksport av prislister

        internal void eksportCorianPrisliste()
        {
            string prislisteUriSomSkalEksporteres = Hjelpeklasser.GlobaleUrier.prislistefilUri();

            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.CheckPathExists = true;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.ShowHelp = false;
            dlg.OverwritePrompt = true;
            dlg.Filter = "Prislistefil|*.data";
            dlg.Title = "Velg hvor prislisten skal lagres";
            dlg.FileName = Path.GetFileNameWithoutExtension(prislisteUriSomSkalEksporteres);

            System.Windows.Forms.DialogResult res = dlg.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(prislisteUriSomSkalEksporteres))
                {
                    try
                    {
                        File.Copy(prislisteUriSomSkalEksporteres, dlg.FileName, true);
                    }
                    catch
                    {
                        MessageBox.Show("Klarte ikke å eksportere prislisten.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Finner ikke prislisten som skal eksporteres.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }


        }

        public bool importPrisliste()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.ShowHelp = false;
            dlg.Filter = "Prislistefil|*.data";
            dlg.Title = "Velg en prisliste som skal importeres";

            System.Windows.Forms.DialogResult res = dlg.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                bool erPrislisteFil = ProduktBeholder.erPrislistefil(dlg.FileName);

                if (!erPrislisteFil)
                {
                    MessageBox.Show("Den valgte filen er ikke en gyldig prislistefil", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                MessageBoxResult utfør = MessageBox.Show("Du er i ferd med å overskrive den gjeldene prislisten.\nDersom du gjør dette vil den tidligere prislisten gå tapt!\n\n Er du sikker på at du vil fortsette?", "Corinor prisforslag", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (utfør != MessageBoxResult.Yes)
                    return false;

                //Ta backup av nåværende prisliste
                string fra = Hjelpeklasser.GlobaleUrier.prislistefilUri(); //Path.Combine(Hjelpeklasser.GlobaleUrier.standardMappe(), Path.GetFileName(dlg.FileName));
                if (File.Exists(fra))
                {
                    string tilBackupmappe = Path.Combine(Hjelpeklasser.GlobaleUrier.standardMappe2(), "Backup");

                    try
                    {
                        if (!Directory.Exists(tilBackupmappe))
                            Directory.CreateDirectory(tilBackupmappe);

                        string tilBackupUrl = Path.Combine(tilBackupmappe, Hjelpeklasser.GlobaleUrier.prislisteNavn);

                        File.Copy(fra, tilBackupUrl, true);
                    }
                    catch
                    {
                        Console.WriteLine("Error: Klarte ikke å ta backup av prislisten.");
                    }
                }
 
                //Importer
                try
                {
                    string til = Hjelpeklasser.GlobaleUrier.prislistefilUri(); //Path.Combine(Hjelpeklasser.GlobaleUrier.standardMappe(), Path.GetFileName(dlg.FileName));
                    File.Copy(dlg.FileName, til, true);
                    return true;
                }
                catch
                {
                    MessageBox.Show("Klarte ikke å importere prislisten.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            return false;

        }
#endregion

        #region Opprettelse av nye TabItems

        internal TabItem nyHeltreTab(Style flatButtonStyle, MainWindow vinduMedCloseTabEventHandler)
        {
            TabItem tab = new TabItem();

            HeltreForslagKontroll forslagKontroll = new HeltreForslagKontroll(data);
            tab.Content = forslagKontroll;

            tab.Header = nyTabHeader(forslagKontroll.ForslagKontrollTittel, flatButtonStyle, vinduMedCloseTabEventHandler);
            return tab;
        }

        internal TabItem nyCorianTab(Style flatButtonStyle, MainWindow vinduMedCloseTabEventHandler)
        {
            TabItem tab = new TabItem();

            CorianForslagKontroll forslagKontroll = new CorianForslagKontroll(data);
            tab.Content = forslagKontroll;

            tab.Header = nyTabHeader(forslagKontroll.ForslagKontrollTittel, flatButtonStyle, vinduMedCloseTabEventHandler);
            return tab;

        }

        private StackPanel nyTabHeader(string tittel, Style flatButtonStyle, MainWindow vinduMedCloseTabEventHandler)
        {
            TextBlock tb = new TextBlock();
            tb.Text = tittel;

            Button b = new Button();
            b.Content = "X";
            b.Margin = new Thickness(4, 0, 0, 0);
            b.Click += vinduMedCloseTabEventHandler.CloseTabItemButton_Click;
            b.Height = 16;
            b.Width = 16;
            b.FontSize = 9;
            b.FontFamily = new FontFamily("Courier");
            b.Focusable = false;
            b.FontWeight = FontWeights.Bold;
            b.Style = flatButtonStyle;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(tb);
            sp.Children.Add(b);

            return sp;

        }
        #endregion




       
    }
}
