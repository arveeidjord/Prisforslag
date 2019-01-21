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
using Corinor.DataAksess;
using System.Collections.ObjectModel;
using Corinor.Modell.CorianProdukt;
using System.ComponentModel;

namespace Corinor.Vinduer.EndrePrislister
{
    /// <summary>
    /// Interaction logic for EndrePrisgrupperVindu.xaml
    /// </summary>
    public partial class EndrePrisgrupperVindu : Window, INotifyPropertyChanged
    {
        DataAksess2 db;
        public List<FargeBeholder> Liste { get; private set; }
        public ObservableCollection<Merknad> Merknader { get; private set; }


        private Farge _fargeIendring = null;
        public Farge FargeIendring
        {
            get { return _fargeIendring; }
            set
            {
                if (_fargeIendring != null)
                    setMerknader(_fargeIendring, merknadMerkeTekstboks.Text);
                _fargeIendring = value;

                OnPropertyChanged("FargeIendring");
            }
        }


        public EndrePrisgrupperVindu(DataAksess2 db)
        {
            InitializeComponent();
            this.db = db;

            if (db.Produktbeholder.Prisgruppesamling.Count == 10)
            {
                for (int i = 11; i < 31; i++)
                {
                    db.Produktbeholder.Prisgruppesamling.Add(new FargeBeholder("Prisgruppe " + i));
                }
            }

            Liste = db.Produktbeholder.Prisgruppesamling;

            prisgruppeListboks.DataContext = this;
            prisgruppeListboks.SelectionChanged += prisgruppeListboks_SelectionChanged;

            nyPrisgruppe.Click += new RoutedEventHandler(nyPrisgruppe_Click);
            slettPrisgruppe.Click += new RoutedEventHandler(slettPrisgruppe_Click);
            endrePrisgruppetittel.Click += new RoutedEventHandler(endrePrisgruppetittel_Click);

            nyFargeKnapp.Click += new RoutedEventHandler(nyFargeKnapp_Click);
            leggTilAlleFargerKnapp.Click += new RoutedEventHandler(leggTilAlleFargerKnapp_Click);
            slettFargeKnapp.Click += new RoutedEventHandler(slettFargeKnapp_Click);

            nyMerknadKnapp.Click += new RoutedEventHandler(nyMerknadKnapp_Click);
            endreMerknadKnapp.Click += new RoutedEventHandler(endreMerknadKnapp_Click);
            slettMerknadKnapp.Click += new RoutedEventHandler(slettMerknadKnapp_Click);

            fargerListboks.SelectionChanged += new SelectionChangedEventHandler(fargerListboks_SelectionChanged);

            lagreKnapp.Click += lagreKnapp_Click;


            Merknader = db.Produktbeholder.Merknader;
            merknaderListboks.DataContext = this;

            fargeTekstboks.DataContext = this;
            merknadMerkeTekstboks.DataContext = this;
        }

        void leggTilAlleFargerKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (! (prisgruppeListboks.SelectedItem is FargeBeholder)) return;

            FargeBeholder merketFargebeholder = prisgruppeListboks.SelectedItem as FargeBeholder;
            List<Farge> nyFargeliste = new List<Farge>();

            foreach (FargeBeholder fb in db.Produktbeholder.Prisgruppesamling)
            {
                if (fb != merketFargebeholder)
                {
                    foreach (Farge farge in fb.Farger)
                    {
                        bool leggTil = true;
                        foreach (Farge fargeIMerketFargebeholder in nyFargeliste)
                        {
                            if (farge.FargeTittel.Trim() == fargeIMerketFargebeholder.FargeTittel.Trim())
                            {
                                leggTil = false;
                                break;
                            }
                        }

                        if (leggTil)
                        {
                            Farge nyFarge = new Farge(farge.FargeTittel, merketFargebeholder);
                            nyFarge.SetMerknader = farge.SetMerknader;

                            foreach (Merknad m in farge.FargeMerknader)
                                nyFarge.FargeMerknader.Add(m);

                            nyFargeliste.Add(farge);
                        }
                    }
                }
            }

            //nyFargeliste.OrderBy(farge => farge.FargeTittel);
            nyFargeliste.Sort(delegate(Farge f1, Farge f2) { return f1.FargeTittel.CompareTo(f2.FargeTittel); });
            foreach (Farge farge in nyFargeliste)
                merketFargebeholder.Farger.Add(farge);

        }

        void slettPrisgruppe_Click(object sender, RoutedEventArgs e)
        {
            if (!(prisgruppeListboks.SelectedItem is FargeBeholder)) return;

            (prisgruppeListboks.SelectedItem as FargeBeholder).Visible = false;

        }

        void nyPrisgruppe_Click(object sender, RoutedEventArgs e)
        {
            foreach (FargeBeholder fargebeholder in db.Produktbeholder.Prisgruppesamling)
            {
                if (!fargebeholder.Visible)
                {
                    fargebeholder.Visible = true;
                    break;
                }

            }
        }

      
        void endrePrisgruppetittel_Click(object sender, RoutedEventArgs e)
        {
            if (!(prisgruppeListboks.SelectedItem is FargeBeholder)) return;

            FargeBeholder merketFargebeholder = prisgruppeListboks.SelectedItem as FargeBeholder;


            string prisgruppenavn = Microsoft.VisualBasic.Interaction.InputBox("Skriv inn det nye navnet på prisgruppen:", "Corinor prisforslag", "");
            if (string.IsNullOrEmpty(prisgruppenavn)) return;

            merketFargebeholder.PrisgruppeNavn = prisgruppenavn;

            //HACK: oppdaterer prisgruppenavnet med å endre datacontext. Bør bruke notify av PrisgruppeNavn
            prisgruppeListboks.DataContext = null;
            prisgruppeListboks.DataContext = this; 

        }

        void fargerListboks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(fargerListboks.SelectedItem is Farge)) return;

            Farge farge = fargerListboks.SelectedItem as Farge;

            merknaderVisning.Text = "";

            foreach (Merknad m in farge.FargeMerknader)
                merknaderVisning.Text += m.MerknadTekst + "\n";

            merknaderVisning.Text = merknaderVisning.Text.Trim();

            FargeIendring = farge;

            fargerListboks.ScrollIntoView(farge);



        }

        void nyMerknadKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (Merknader == null) return;

            string merknadMerke = Microsoft.VisualBasic.Interaction.InputBox("Skriv inn merket (feks: *** eller #) på merknaden:", "Corinor prisforslag", "");
            if (string.IsNullOrEmpty(merknadMerke)) return;

            string merknadTekst = Microsoft.VisualBasic.Interaction.InputBox("Skriv inn merknaden:", "Corinor prisforslag", "");
            if (string.IsNullOrEmpty(merknadTekst)) return;


            Merknader.Add(new Merknad(merknadMerke, merknadTekst));

        }

        void endreMerknadKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (!(merknaderListboks.SelectedItem is Merknad)) return;

            Merknad merketMerknad = merknaderListboks.SelectedItem as Merknad;

            string merknadTekst = Microsoft.VisualBasic.Interaction.InputBox("Skriv inn merknaden:", "Corinor prisforslag", merketMerknad.MerknadTekst);
            if (string.IsNullOrEmpty(merknadTekst)) return;

            merketMerknad.MerknadTekst = merknadTekst;

            //HACK: Oppdaterer Listboxen ved bruk av DataContext og ikke Notify
            merknaderListboks.DataContext = null;
            merknaderListboks.DataContext = this;

        }


        void slettMerknadKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (!(merknaderListboks.SelectedItem is Merknad)) return;

            Merknad merketMerknad = merknaderListboks.SelectedItem as Merknad;

            foreach (FargeBeholder fargebeholder in db.Produktbeholder.Prisgruppesamling)
            { 
                foreach (Farge farge in fargebeholder.Farger)
                    if (farge.FargeMerknader.Contains(merketMerknad))
                    {
                        MessageBox.Show("Kan ikke slette merknaden siden den er knyttet til en farge.", "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }
            }

            Merknader.Remove(merketMerknad);
        }

        void slettFargeKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (prisgruppeListboks.SelectedItem == null || !(prisgruppeListboks.SelectedItem is FargeBeholder))
                return;

            int selIndex = fargerListboks.SelectedIndex;

            FargeBeholder fb = (prisgruppeListboks.SelectedItem as FargeBeholder);

            int ant = fargerListboks.SelectedItems.Count;
            for (int i = 0; i < ant; i++)
            {
                if (fargerListboks.SelectedItems[0] is Farge)
                    fb.Farger.Remove(fargerListboks.SelectedItems[0] as Farge);
            }



            if (selIndex < fargerListboks.Items.Count)
                fargerListboks.SelectedIndex = selIndex;
            else
                fargerListboks.SelectedIndex = fargerListboks.Items.Count - 1;
        }
        
        int nrFarge = 1;
        void nyFargeKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (prisgruppeListboks.SelectedItem == null || !(prisgruppeListboks.SelectedItem is FargeBeholder))
                return;

            //string prisgruppenavn = "";
            //if (prisgruppeListboks.SelectedItem is FargeBeholder)
            //    prisgruppenavn = (prisgruppeListboks.SelectedItem as FargeBeholder).PrisgruppeNavn;
            FargeBeholder fb = (prisgruppeListboks.SelectedItem as FargeBeholder);
            int index = fargerListboks.SelectedIndex + 1;
            if (fargerListboks.SelectedIndex == -1)
                index = fargerListboks.Items.Count;

            Farge nyFarge = new Farge("Farge" + nrFarge++, prisgruppeListboks.SelectedItem as FargeBeholder);
            fb.Farger.Insert(index, nyFarge);

            fargerListboks.ScrollIntoView(nyFarge);
        }

        void prisgruppeListboks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(prisgruppeListboks.SelectedItem is FargeBeholder))
                return;

            FargeBeholder fargeBeholder = prisgruppeListboks.SelectedItem as FargeBeholder;
            fargerListboks.DataContext = fargeBeholder;
            
        }

        void lagreKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                
        }

        private void setMerknader(Farge farge, string tekst)
        {
            farge.FargeMerknader.Clear();

            string[] merker = tekst.Split(';');

            string nyTekst = "";
            if (merker != null)
            {
                foreach (Merknad alleMerke in db.Produktbeholder.Merknader)
                {
                    foreach (string ettmerke in merker)
                        if (!string.IsNullOrWhiteSpace(ettmerke) && alleMerke.MerknadMerke == ettmerke.Trim())
                        {
                            farge.FargeMerknader.Add(alleMerke);
                            nyTekst += alleMerke.MerknadMerke + ";";
                            break;
                        }
                }

            }
            farge.SetMerknader = nyTekst.TrimEnd(';');
        }

        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox t = sender as TextBox;

        //    ListViewItem lvitem = Hjelpeklasser.VisualTree.getParentKontroll<ListViewItem>(t as DependencyObject);
        //    if (lvitem == null) return;
        //    Farge farge = lvitem.Content as Farge;
        //    if (farge == null) return;

        //    farge.FargeMerknader.Clear();

        //    string[] merker = t.Text.Split(';');

        //    string nyTekst = "";
        //    if (merker != null)
        //    {
        //        foreach (Merknad alleMerke in db.Produktbeholder.Merknader)
        //        {
        //            foreach (string ettmerke in merker)
        //                if (!string.IsNullOrWhiteSpace(ettmerke) && alleMerke.MerknadMerke == ettmerke.Trim())
        //                {
        //                    farge.FargeMerknader.Add(alleMerke);
        //                    nyTekst += alleMerke.MerknadMerke + ";";
        //                    break;
        //                }
        //        }

        //    }
        //    farge.SetMerknader = nyTekst.TrimEnd(';');
        //}


        private void tykkelseTekstboks_KeyUp(object sender, KeyEventArgs e)
        {
            FrameworkElement f = sender as FrameworkElement;
            e.Handled = true;


            switch (e.Key)
            {
                case Key.Down:
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
                 

                        fargerListboks.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                        if (fargerListboks.SelectedIndex < fargerListboks.Items.Count - 1)
                            fargerListboks.SelectedIndex += 1;

                        f.Focus();

                        return;
                case Key.Up:
                        fargerListboks.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                        if (fargerListboks.SelectedIndex > 0)
                            fargerListboks.SelectedIndex -= 1;
                        f.Focus();

                        return;

            }

            if (f == fargeTekstboks)
            {
                if (e.Key == Key.Right) merknadMerkeTekstboks.Focus();
            }
            else if (f == merknadMerkeTekstboks)
            {
                if (e.Key == Key.Left) fargeTekstboks.Focus();

            }


        }


        private void tykkelseTekstboks_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            t.SelectAll();
        }



        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
