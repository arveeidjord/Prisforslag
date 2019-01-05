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
using Corinor.Modell.Heltre;
using Corinor.DataAksess;
using System.ComponentModel;

namespace Corinor.Vinduer.EndrePrislister
{
    /// <summary>
    /// Interaction logic for EndreHeltrePrisliste.xaml
    /// </summary>
    public partial class EndreHeltrePrisliste : Window, INotifyPropertyChanged
    {
        public ObservableCollection<HeltreProdukt> Liste { get; private set; }

        private HeltreProdukt _produktIendring = null;
        public HeltreProdukt produktIendring
        { get { return _produktIendring; }
            set { _produktIendring = value;
            OnPropertyChanged("produktIendring");
            }
        }

        DataAksess2 db;

        public EndreHeltrePrisliste(DataAksess2 db)
        {
            InitializeComponent();
            this.db = db;
            this.Closing += new System.ComponentModel.CancelEventHandler(EndreHeltrePrisliste_Closing);

            this.nyttProduktKnapp.Click += new RoutedEventHandler(nyttProduktKnapp_Click);
            this.slettKnapp.Click += new RoutedEventHandler(slettKnapp_Click);
            kopierProduktKnapp.Click += new RoutedEventHandler(kopierProduktKnapp_Click);

            endreKnapp.Click += new RoutedEventHandler(endreKnapp_Click);
            endreTreslagKnapp.Click += new RoutedEventHandler(endreTreslagKnapp_Click);
            endreTykkelseKnapp.Click += new RoutedEventHandler(endreTykkelseKnapp_Click);
            endreTypeKnapp.Click += new RoutedEventHandler(endreTypeKnapp_Click);
            endredybdeKnapp.Click += new RoutedEventHandler(endredybdeKnapp_Click);
            endrePrisKnapp.Click += new RoutedEventHandler(endrePrisKnapp_Click);

            oppKnapp.Click += new RoutedEventHandler(oppKnapp_Click);
            nedKnapp.Click += new RoutedEventHandler(nedKnapp_Click);

            this.lagreKnapp.Click += new RoutedEventHandler(lagreKnapp_Click);
            this.avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);

            Liste = db.Produktbeholder.HeltreProduktliste;
            if (Liste == null)
                Liste = new ObservableCollection<HeltreProdukt>();

            //for (int i = 0; i < 1000; i++)
            //    Liste.Add(new HeltreProdukt("Tykkelse", "Treslag", "type", "dybde/størrelse", 0, HeltreProdukt.PerPrisType.perAntall));

            listview.SelectionChanged += new SelectionChangedEventHandler(listview_SelectionChanged);
            
            listview.DataContext = this;
            tykkelseTekstboks.DataContext = this;
            typeTekstboks.DataContext = this;
            treslagTekstboks.DataContext = this;
            størrelseTekstboks.DataContext = this;
            prisTekstboks.DataContext = this;
            pristypeVelger.DataContext = this;
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

     
        void endreTykkelseKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is HeltreProdukt)) return;

            string tykkelse = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn tykkelsen de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as HeltreProdukt).Tykkelse);
            if (string.IsNullOrEmpty(tykkelse)) return;

            foreach (HeltreProdukt produkt in listview.SelectedItems)
                produkt.Tykkelse = tykkelse;
        }


        void endreTreslagKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is HeltreProdukt)) return;

            string treslag = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn treslaget de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as HeltreProdukt).Treslag);
            if (string.IsNullOrEmpty(treslag)) return;

            foreach (HeltreProdukt produkt in listview.SelectedItems)
                produkt.Treslag = treslag;
        }


        void endreTypeKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is HeltreProdukt)) return;

            string type = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn typen de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as HeltreProdukt).Type);
            if (string.IsNullOrEmpty(type)) return;

            foreach (HeltreProdukt produkt in listview.SelectedItems)
                produkt.Type = type;
        }


        void endredybdeKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is HeltreProdukt)) return;

            string dybde = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn dybdeintervall/størrelse de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as HeltreProdukt).DybdeintervallStørrelse);
            if (string.IsNullOrEmpty(dybde)) return;

            foreach (HeltreProdukt produkt in listview.SelectedItems)
                produkt.DybdeintervallStørrelse = dybde;
        }


        void endrePrisKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;
            if (!(listview.SelectedItems[0] is HeltreProdukt)) return;

            string pris = Microsoft.VisualBasic.Interaction.InputBox(string.Format("Du har valgt å endre {0} elementer.\nSkriv inn prisen de merkede elementen skal få:", listview.SelectedItems.Count), "Corinor prisforslag", (listview.SelectedItems[0] as HeltreProdukt).Pris.ToString());
            if (string.IsNullOrEmpty(pris)) return;

            double prisTall = -1;
            double.TryParse(pris, out prisTall);
            if (prisTall < 0) return;

            foreach (HeltreProdukt produkt in listview.SelectedItems)
                produkt.Pris = prisTall;
        }




        void kopierProduktKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;

            int i = 0;
            foreach (HeltreProdukt produkt in listview.SelectedItems)
            {
                HeltreProdukt kopiProdukt = new HeltreProdukt(produkt.Tykkelse, produkt.Treslag, produkt.Type, produkt.DybdeintervallStørrelse, 0, produkt.PrisPer);
                Liste.Insert(i++, kopiProdukt);
                //Liste.Add(kopiProdukt);
            }
        }

        void nedKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;

            for (int i = listview.Items.Count-1; i >= 0 ; i--)
            {
                foreach (HeltreProdukt produkt in listview.SelectedItems)
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

        void oppKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;

            for (int i = 0; i < listview.Items.Count; i++)
            {
                foreach (HeltreProdukt produkt in listview.SelectedItems)
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

        void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            produktIendring = listview.SelectedItem as HeltreProdukt;

            listview.ScrollIntoView(listview.SelectedItem as HeltreProdukt);
        }

        void EndreHeltrePrisliste_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lagreKnapp.Focus(); //for at alle data skal bli oppdatert

            if (this.DialogResult != true)
            {
                MessageBoxResult res = MessageBox.Show("Vil du lagre endringene i prislisten?", "Corinor prisforslag", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                    db.Produktbeholder.saveData();
            }
        }

        void slettKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItems.Count == 0) return;

            int ant = listview.SelectedItems.Count;
            for (int i = 0; i < ant; i++) 
                Liste.Remove(listview.SelectedItems[0] as HeltreProdukt);
        }

        void nyttProduktKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedItem != null && listview.SelectedItem is HeltreProdukt)
            {
                HeltreProdukt merketProdukt = listview.SelectedItem as HeltreProdukt;
                Liste.Insert(listview.SelectedIndex + 1, new HeltreProdukt(merketProdukt.Tykkelse, merketProdukt.Treslag, merketProdukt.Type, merketProdukt.DybdeintervallStørrelse, 0, merketProdukt.PrisPer));
            }
            else
                Liste.Add(new HeltreProdukt("Tykkelse", "Treslag", "type", "dybde/størrelse", 0, HeltreProdukt.PerPrisType.perAntall));
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

   

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
                    if (f != pristypeVelger)
                    {
                        listview.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                        if (listview.SelectedIndex < listview.Items.Count -1)
                            listview.SelectedIndex += 1;

                        f.Focus();

                        return;
                    }
                    break;
                case Key.Up:

                    if (f != pristypeVelger)
                    {
                        listview.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
                        if (listview.SelectedIndex > 0)
                            listview.SelectedIndex -= 1;
                        f.Focus();

                        return;
                    }

                    break;
            }



            //if (e.Key == Key.Down && )
            //{

               
            //}
            //else if (e.Key == Key.Up && f != pristypeVelger)
            //{
            //    listview.Focus();//Fjerner focus fra den tekstboks som er aktiv sånn at data fra den blir oppdatert
            //    if (listview.SelectedIndex > 0)
            //        listview.SelectedIndex -= 1;
            //    f.Focus();

            //    return;
            //}
            
            if (f == tykkelseTekstboks)
            {
                if (e.Key == Key.Right) treslagTekstboks.Focus();
                //if (e.Key == Key.Left) 
                
            }
            else if (f == treslagTekstboks)
            {
                if (e.Key == Key.Right) typeTekstboks.Focus();
                if (e.Key == Key.Left) tykkelseTekstboks.Focus();


            }
            else if (f == typeTekstboks)
            {
                if (e.Key == Key.Right) størrelseTekstboks.Focus();
                if (e.Key == Key.Left) treslagTekstboks.Focus();
            
            }
       
            else if (f == størrelseTekstboks)
            {
                if (e.Key == Key.Right) prisTekstboks.Focus(); //pristypeVelger.Focus();
                if (e.Key == Key.Left) typeTekstboks.Focus();

            
            }
            //else if (f == pristypeVelger)
            //{
            //    if (e.Key == Key.Right) prisTekstboks.Focus();
            //    if (e.Key == Key.Left) størrelseTekstboks.Focus();

            
            //}
            else if (f == prisTekstboks)
            {
                if (e.Key == Key.Left) størrelseTekstboks.Focus();//pristypeVelger.Focus();
            
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
