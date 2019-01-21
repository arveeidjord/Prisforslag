using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Corinor.Modell.CorianProdukt;
using Corinor.DataAksess;
using System.ComponentModel;

namespace Corinor.Vinduer.EndrePrislister
{
    /// <summary>
    /// Interaction logic for EndreCorianPrisliste.xaml
    /// </summary>
    public partial class EndreCorianPrisliste : Window, INotifyPropertyChanged
    {
        public ObservableCollection<CorianProdukt> Liste { get; private set; }
        //CollectionViewSource cvs = null;
        DataAksess2 db;

        private CorianProdukt _produktIendring = null;
        public CorianProdukt ProduktIendring
        { 
            get { return _produktIendring; }
            set { 
                _produktIendring = value;

                if (_produktIendring == null || _produktIendring.Prisgruppe == null || _produktIendring.Prisgruppe.Avhengighet == null)
                {
                    avhengighetPrisTekstboks.Visibility = System.Windows.Visibility.Hidden;
                    pristypeVelger.Visibility = System.Windows.Visibility.Hidden;
                    kategoriTekstboks.IsEnabled = true;
                }
                else
                {
                    avhengighetPrisTekstboks.Visibility = System.Windows.Visibility.Visible;
                    pristypeVelger.Visibility = System.Windows.Visibility.Visible;
                    kategoriTekstboks.IsEnabled = true;
                }

                OnPropertyChanged("ProduktIendring");
            }
        }

        public EndreCorianPrisliste(DataAksess2 db)
        {
            InitializeComponent();
            this.db = db;

            this.lagreKnapp.Click += new RoutedEventHandler(lagreKnapp_Click);
            this.avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);

            this.Closing += new System.ComponentModel.CancelEventHandler(EndreCorianPrisliste_Closing);

            this.listview.SelectionChanged += new SelectionChangedEventHandler(listview_SelectionChanged);
            this.nyttProduktKnapp.Click += new RoutedEventHandler(nyttProduktKnapp_Click);
            this.nyttUnderproduktKnapp.Click += new RoutedEventHandler(nyttUnderproduktKnapp_Click);

            endreKnapp.Click += new RoutedEventHandler(endreKnapp_Click);
            endreKategoriKnapp.Click += new RoutedEventHandler(endreKategoriKnapp_Click);
            endreProduktKnapp.Click += new RoutedEventHandler(endreProduktKnapp_Click);
            endrePrisKnapp.Click += new RoutedEventHandler(endrePrisKnapp_Click);
            endrePrisPrisgrupperKnapp.Click += new RoutedEventHandler(endrePrisPrisgrupperKnapp_Click);

            this.oppKnapp.Click += new RoutedEventHandler(oppKnapp_Click);
            this.nedKnapp.Click += new RoutedEventHandler(nedKnapp_Click);

            this.slettKnapp.Click += new RoutedEventHandler(slettKnapp_Click);
            this.endrePrisgrupperKnapp.Click += new RoutedEventHandler(endrePrisgrupperKnapp_Click);

            this.cmbPrisgruppe.SelectionChanged += new SelectionChangedEventHandler(cmbPrisgruppe_SelectionChanged);

            Liste = db.Produktbeholder.ProduktListe;
            if (Liste == null)
                Liste = new ObservableCollection<CorianProdukt>();

            listview.DataContext = this;

            //cvs = new CollectionViewSource();
            //cvs.Source = Liste;
            //cvs.GroupDescriptions.Add(new PropertyGroupDescription("ProduktKategori"));
            //listview.ItemsSource = cvs.View;
            listview.ItemsSource = Liste;

            oppdagerPrisliseVelger();

            kategoriTekstboks.DataContext = this;
            produktTekstboks.DataContext = this;
            delingVelger.DataContext = this;
            avhengighetPrisTekstboks.DataContext = this;
            pristypeVelger.DataContext = this;
            prisTekstboks.DataContext = this;
            prisPerVelger.DataContext = this;

        }

        void endreKnapp_Click(object sender, RoutedEventArgs e)
        {
            Button but = (Button)sender;
            if (but == null || but.ContextMenu == null) return;

            if (!but.ContextMenu.IsOpen)
            {
                but.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                but.ContextMenu.PlacementTarget = but;

                but.ContextMenu.IsOpen = true;
            }
        }

        void endrePrisKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is CorianProdukt)) return;
            if ((listview.SelectedItems[0] as CorianProdukt).Prisgruppe == null) return;

            string pris = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn prisen de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as CorianProdukt).Prisgruppe.Prisgrunnlag.ToString());
            if (string.IsNullOrEmpty(pris)) return;

            double prisTall = -1;
            double.TryParse(pris, out prisTall);
            if (prisTall < 0) return;

            foreach (CorianProdukt produkt in listview.SelectedItems)
                if (produkt.Prisgruppe != null) 
                    produkt.Prisgruppe.Prisgrunnlag = prisTall;
        }

        void endrePrisPrisgrupperKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is CorianProdukt)) return;
            if ((listview.SelectedItems[0] as CorianProdukt).Prisgruppe == null) return;

            var a = listview.SelectedItems.Cast<CorianProdukt>().ToList();
            var endrePrisIflerePrisgrupperVindu = new EndrePrisIflerePrisgrupperVindu(a, cmbPrisgruppe.SelectedItem as FargeBeholder, db);
            endrePrisIflerePrisgrupperVindu.ShowDialog();
        }

        void endreKategoriKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is CorianProdukt)) return;

            string kategori = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn kategorinavnet de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as CorianProdukt).ProduktKategori);
            if (string.IsNullOrEmpty(kategori)) return;

            foreach (CorianProdukt produkt in listview.SelectedItems)
            {
                if (produkt.Prisgrupper[0].Avhengighet == null)
                {
                    produkt.ProduktKategori = kategori;
                    oppdaterKategoriTilUnderprodukter(kategori, produkt);
                }
            }
        }


        void endreProduktKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is CorianProdukt)) return;

            string produktnavn = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn produktnavnet de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as CorianProdukt).Navn);
            if (string.IsNullOrEmpty(produktnavn)) return;

            foreach (CorianProdukt produkt in listview.SelectedItems)
                produkt.Navn = produktnavn;

        }

  
        private void oppdagerPrisliseVelger()
        {
            cmbPrisgruppe.Items.Clear();
            foreach (FargeBeholder fb in db.Produktbeholder.Prisgruppesamling)
                if (fb.Visible)
                    cmbPrisgruppe.Items.Add(fb);

            if (cmbPrisgruppe.Items.Count > 0) cmbPrisgruppe.SelectedIndex = 0;
        }

        void oppKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;

            for (int i = 0; i < listview.Items.Count; i++)
            {
                foreach (CorianProdukt produkt in listview.SelectedItems)
                {
                    if (listview.Items[i] == produkt)
                    {
                        int newIndex = i - 1;
                        if (newIndex < 0) return;

                        Liste.Move(i, newIndex);
                        break;
                    }
                }

            }
        }

        void nedKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;

            for (int i = listview.Items.Count - 1; i >= 0; i--)
            {
                foreach (CorianProdukt produkt in listview.SelectedItems)
                {
                    if (listview.Items[i] == produkt)
                    {
                        int newIndex = i + 1;
                        if (newIndex >= listview.Items.Count) return;

                        Liste.Move(i, newIndex);
                        break;
                    }
                }

            }
        }

    
       

        void cmbPrisgruppe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedItem == null || !(cmb.SelectedItem is FargeBeholder))
                return;

            prisgruppeColumn.Header = (cmbPrisgruppe.SelectedItem as FargeBeholder).PrisgruppeNavn;
            FargeBeholder merketFargebeholder = cmb.SelectedItem as FargeBeholder;

            //Ny logikk for å passe på at prisgrupper blir opprettet

            var antallIkkeFunnet = 0;
            CorianProdukt forrigeProdukt = null;
            foreach (CorianProdukt produkt in Liste)
            {
                var funnet = false;
                foreach (CorianPrisgruppeProdukt prisgruppe in produkt.Prisgrupper)
                {
                    if (prisgruppe != null && prisgruppe.Farger == merketFargebeholder)
                    {
                        produkt.Prisgruppe = prisgruppe;
                        funnet = true;
                        break;
                        
                    }

                }//Prisgrupper

                if (!funnet)
                {
                    var f = produkt.Prisgrupper.ToList();

                    var prisgruppe = produkt.Prisgrupper[0].Avhengighet;
                    if (forrigeProdukt != null && produkt.Prisgrupper[0].Avhengighet != null)
                    {
                        prisgruppe = forrigeProdukt.Prisgrupper.FirstOrDefault(x => x.Farger == merketFargebeholder);

                        if (prisgruppe == null)
                            MessageBox.Show("Prisgruppe er NULL");
                    }

                    var nyGruppe = new CorianPrisgruppeProdukt(0,
                            prisgruppe, //TODO: Skal avhengighet være samme for alle prisgruper?
                            produkt.Prisgrupper[0].prisType,
                            merketFargebeholder);


                    f.Add(nyGruppe);

                    produkt.Prisgrupper = f.ToArray();
                    produkt.Prisgruppe = nyGruppe;
                    antallIkkeFunnet++;

                }

                forrigeProdukt = produkt;
            }//Liste

            if (antallIkkeFunnet > 0)
                MessageBox.Show("Fant ikke alle prisgrupper. Mangler på antall produkter: " + antallIkkeFunnet);



                //prisTypeVelger
                //prisgrunnlagTekstboks

        }

        void endrePrisgrupperKnapp_Click(object sender, RoutedEventArgs e)
        {
            EndrePrisgrupperVindu endrePrisgrupperVindu = new EndrePrisgrupperVindu(db);
            endrePrisgrupperVindu.Owner = this;
            endrePrisgrupperVindu.ShowDialog();

            oppdagerPrisliseVelger();
        }

        void EndreCorianPrisliste_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lagreKnapp.Focus(); //for at alle data skal bli oppdatert

            if (this.DialogResult != true)
            {
                MessageBoxResult res = MessageBox.Show("Vil du lagre endringene i prislisten?", "Corinor prisforslag", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                    db.Produktbeholder.saveData();
            }

            Liste = null;
            db = null;

           

        }

        void slettKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;

            int ant = listview.SelectedItems.Count;
            for (int i = 0; i < ant; i++)
                if (listview.SelectedItems[0] is CorianProdukt) Liste.Remove(listview.SelectedItems[0] as CorianProdukt);

            //lagreKnapp.IsEnabled = true;
        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void lagreKnapp_Click(object sender, RoutedEventArgs e)
        {
            lagreKnapp.Focus(); //for at alle data skal bli oppdatert
            db.Produktbeholder.saveData();
            this.DialogResult = true;
            this.Close();
        }

        void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listview.SelectedItem != null && (listview.SelectedItem is CorianProdukt) 
                && (listview.SelectedItem as CorianProdukt).Prisgrupper[0].prisType == CorianPrisgruppeProdukt.PrisType.Normal)
                nyttUnderproduktKnapp.IsEnabled = true;
            else
                nyttUnderproduktKnapp.IsEnabled = false;

            CorianProdukt corianProduktData = listview.SelectedItem as CorianProdukt;
            ProduktIendring = corianProduktData;

            listview.ScrollIntoView(corianProduktData);
        }

 

        void nyttProduktKnapp_Click(object sender, RoutedEventArgs e)
        {            
            CorianProdukt c = new CorianProdukt("Kategori", "Produktnavn", Modell.Produkt.DelingType.CorianKumHeltreKum);

            CorianProdukt merketProdukt = listview.SelectedItem as CorianProdukt;
            if (merketProdukt != null)
            {
                c.ProduktKategori = merketProdukt.ProduktKategori;
                c.Navn = merketProdukt.Navn;
                c.Deling = merketProdukt.Deling;
            }

            for (int i = 0; i < db.Produktbeholder.Prisgruppesamling.Count; i++)
                c.Prisgrupper[i] = new CorianPrisgruppeProdukt(0, null, CorianPrisgruppeProdukt.PrisType.Normal, db.Produktbeholder.Prisgruppesamling[i]);

            //Setter rett prisgruppe
            FargeBeholder fargebeholder = cmbPrisgruppe.SelectedItem as FargeBeholder;
            foreach (CorianPrisgruppeProdukt prisgruppe in c.Prisgrupper)
            {
                if (prisgruppe.Farger == fargebeholder)
                {
                    c.Prisgruppe = prisgruppe;
                    break;
                }
            }

            int toIndex = listview.SelectedIndex + 1;

            while (toIndex < listview.Items.Count && (listview.Items[toIndex] as CorianProdukt).Prisgruppe.Avhengighet != null)
                toIndex++;

            Liste.Insert(toIndex, c);

        }

        void nyttUnderproduktKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItem == null || !(listview.SelectedItem is CorianProdukt))
                return; 

            CorianProdukt merketProdukt = listview.SelectedItem as CorianProdukt;

            if (merketProdukt.Prisgrupper[0].Avhengighet != null) return;

            CorianProdukt c = new CorianProdukt(merketProdukt.ProduktKategori, "Produktnavn", merketProdukt.Deling);
            c.PrisPer = merketProdukt.PrisPer;

            for (int i = 0; i < db.Produktbeholder.Prisgruppesamling.Count; i++)
                c.Prisgrupper[i] = new CorianPrisgruppeProdukt(0, merketProdukt.Prisgrupper[i], CorianPrisgruppeProdukt.PrisType.Adder, db.Produktbeholder.Prisgruppesamling[i]);

            //Setter rett prisgruppe
            FargeBeholder fargebeholder = cmbPrisgruppe.SelectedItem as FargeBeholder;
            foreach (CorianPrisgruppeProdukt prisgruppe in c.Prisgrupper)
            {
                if (prisgruppe.Farger == fargebeholder)
                {
                    c.Prisgruppe = prisgruppe;
                    break;
                }
            }

            Liste.Insert(listview.SelectedIndex + 1, c);


        }


        private void tykkelseTekstboks_KeyUp(object sender, KeyEventArgs e)
        {
            FrameworkElement f = sender as FrameworkElement;
            e.Handled = true;


            switch (e.Key)
            {
                case Key.Down:
                case Key.Enter:
                case Key.F1:
                case Key.F2:
                case Key.F3:
                case Key.F4:
                case Key.F5:
                case Key.F6:
                case Key.F7:
                case Key.F8:
                case Key.F9:
                case Key.F10:
                case Key.F11:
                case Key.F12:
                    if (e.Key != Key.Down || (f != delingVelger && f != pristypeVelger && f != prisPerVelger))
                    {

                        listview.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                        if (listview.SelectedIndex < listview.Items.Count - 1)
                            listview.SelectedIndex += 1;

                        f.Focus();

                        return;
                    //    listview.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                    //    if (listview.SelectedIndex < listview.Items.Count - 1)
                    //        listview.SelectedIndex += 1;

                    //    f.Focus();

                    //    return;
                    }
                    break;
                case Key.Up:

                    if (f != delingVelger && f != pristypeVelger && f != prisPerVelger)
                    {
                        listview.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                        if (listview.SelectedIndex > 0)
                            listview.SelectedIndex -= 1;
                        f.Focus();

                        return;
                        //listview.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                        //if (listview.SelectedIndex > 0)
                        //    listview.SelectedIndex -= 1;
                        //f.Focus();

                        //return;
                    }

                    break;
            }

            if (f == kategoriTekstboks)
            {
                if (e.Key == Key.Right) produktTekstboks.Focus();
            }
            else if (f == produktTekstboks)
            {
                if (e.Key == Key.Right) prisTekstboks.Focus();
                if (e.Key == Key.Left) kategoriTekstboks.Focus();

            }
            else if (f == prisTekstboks)
            {
                if (e.Key == Key.Left) produktTekstboks.Focus();


            }


        }

        private void tykkelseTekstboks_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            t.SelectAll();
        }



        //private void KategoriTextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    //if (cvs == null) return;
        //    //cvs.View.Refresh();
        //}

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void kategoriTekstboks_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ProduktIendring == null) return;
            TextBox t = sender as TextBox;

            oppdaterKategoriTilUnderprodukter(t.Text, ProduktIendring);
        }


        private void oppdaterKategoriTilUnderprodukter(string nyKategori, CorianProdukt produktSomEndres)
        {
            foreach (CorianProdukt produkt in Liste)
            {
                if (produkt.Prisgrupper[0].Avhengighet != null && produkt.Prisgrupper[0].Avhengighet == produktSomEndres.Prisgrupper[0])
                    produkt.ProduktKategori = nyKategori;
            }
        }



    }
}
