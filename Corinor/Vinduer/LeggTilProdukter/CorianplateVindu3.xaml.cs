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
using Corinor.Modell.CorianProdukt;
using System.Collections.ObjectModel;
using Corinor.DataAksess;
using Corinor.Presentasjon;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using Corinor.Modell;

namespace Corinor.Vinduer
{
    /// <summary>
    /// Interaction logic for CorianplateVindu3.xaml
    /// </summary>
    public partial class CorianplateVindu3 : Window
    {
        DataAksess2 db;
        
        public ObservableCollection<CorianProdukt> Liste { get; private set; }
        public KontrollProduktPresentasjon Produkt { get; private set; }

        bool _visPrisgruppeVelger = false;
        public string HuskFarge = "";

        public CorianplateVindu3(DataAksess2 db, Produkt.DelingType deling, Produkt.DelingType deling2, bool visPrisgruppeVelger, string huskFarge)
        {
            InitializeComponent();
            this.db = db;
            this.HuskFarge = huskFarge;
            Liste = new ObservableCollection<CorianProdukt>();

            foreach (CorianProdukt p in db.Produktbeholder.ProduktListe)
                if (p.Deling == deling || p.Deling == deling2)
                    Liste.Add(p);

            if (!visPrisgruppeVelger) initFargevelger();
            else initPrisgruppevelger();

            _visPrisgruppeVelger = visPrisgruppeVelger;

            avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);
            LeggTilKnapp.Click += new RoutedEventHandler(LeggTilKnapp_Click);

            listview.DataContext = this;

            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = Liste;
            cvs.GroupDescriptions.Add(new PropertyGroupDescription("ProduktKategori"));
            listview.ItemsSource = cvs.View;




        }

        void initFargevelger()
        {
            cmbFargevelger2.SelectionChanged += new SelectionChangedEventHandler(cmbFargevelger2_SelectionChanged);

            List<Farge> farger = new List<Farge>();
            foreach (CorianProdukt produkt in Liste)
            {
                foreach (CorianPrisgruppeProdukt prisgruppe in produkt.Prisgrupper)
                {
                    if (prisgruppe != null && prisgruppe.Farger.Visible)
                        foreach (Farge farge in prisgruppe.Farger.Farger)
                        {
                            bool fargeFinnes = false;
                            foreach (Farge f in farger)
                                if (f.FargeTittel == farge.FargeTittel)
                                {
                                    fargeFinnes = true;
                                    break;
                                }

                            if (!fargeFinnes)
                                farger.Add(farge);
                        }
                }
            }

            cmbFargevelger2.Items.Clear();
            foreach (Farge farge in farger)
                cmbFargevelger2.Items.Add(farge);

            cmbFargevelger2.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("SortingTittel", System.ComponentModel.ListSortDirection.Ascending));

            if (string.IsNullOrEmpty(HuskFarge))
                HuskFarge = "bone";

            //Velger Bone som standardfarge. Sånn at det ser fint ut. Alltid priser overalt
            if (cmbFargevelger2.Items.Count > 0)
            {
                foreach (Farge f in cmbFargevelger2.Items)
                    if (f.FargeTittel.Trim().ToLower() == HuskFarge.ToLower()) { cmbFargevelger2.SelectedItem = f; break; }
            }

            velgFargeLabel.Visibility = System.Windows.Visibility.Visible;
            cmbFargevelger2.Visibility = System.Windows.Visibility.Visible;

            
           
        }

        void initPrisgruppevelger()
        {
            cmbPrisgruppeVelger.SelectionChanged += new SelectionChangedEventHandler(cmbPrisgruppeVelger_SelectionChanged);
            cmbPrisgruppeVelger.Items.Clear();

            foreach (CorianProdukt produkt in Liste)
            {
                foreach (CorianPrisgruppeProdukt prisgruppe in produkt.Prisgrupper)
                {
                    if (prisgruppe != null && prisgruppe.Farger.Visible)
                    {
                        if (prisgruppe.Pris > 0 && !cmbPrisgruppeVelger.Items.Contains(prisgruppe.Farger))
                            cmbPrisgruppeVelger.Items.Add(prisgruppe.Farger);
                    }

                }
            }
            if (cmbPrisgruppeVelger.Items.Count > 0)
            {
                if (string.IsNullOrEmpty(HuskFarge))
                    cmbPrisgruppeVelger.SelectedIndex = 0;
                else
                {
                    var allePrisgrupper = cmbPrisgruppeVelger.Items.Cast<FargeBeholder>().ToList();

                    var valg = allePrisgrupper.FirstOrDefault(x => x.PrisgruppeNavn == HuskFarge);

                    if (valg != null)
                        cmbPrisgruppeVelger.SelectedItem = valg;
                    else
                        cmbPrisgruppeVelger.SelectedIndex = 0;
                           
                }
                
                    
            }

            

            velgPrisgruppeLabel.Visibility = System.Windows.Visibility.Visible;
            cmbPrisgruppeVelger.Visibility = System.Windows.Visibility.Visible;
        }

        void cmbFargevelger2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedItem == null || !(cmb.SelectedItem is Farge)) 
                return;

            Farge valgtFarge = cmb.SelectedItem as Farge;

            foreach (CorianProdukt produkt in Liste)
            {
                produkt.Prisgruppe = null;
                bool doBreak = false;
                foreach (CorianPrisgruppeProdukt prisgruppe in produkt.Prisgrupper)
                {
                    if (prisgruppe != null)
                        foreach (Farge farge in prisgruppe.Farger.Farger)
                        {
                            //string s = prisgruppe.Farger.PrisgruppeNavn + ": " + farge;
                            if (valgtFarge.FargeTittel == farge.FargeTittel && prisgruppe.Pris > 0)
                            //if (valgtFarge == farge)
                            {
                                produkt.Prisgruppe = prisgruppe;
                                doBreak = true;
                                break;
                            }
                        }

                    if (doBreak)
                        break;
                }//Prisgrupper

                

            }//Liste


            if (sistePrisKlikket != null)
                PrisRadioButton_Click(sistePrisKlikket, new RoutedEventArgs());
        }

        void cmbPrisgruppeVelger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedItem == null || !(cmb.SelectedItem is FargeBeholder))
                return;

            FargeBeholder valgtFargeBeholder = cmb.SelectedItem as FargeBeholder;

            foreach (CorianProdukt produkt in Liste)
            {
                //bool doBreak = false;
                foreach (CorianPrisgruppeProdukt prisgruppe in produkt.Prisgrupper)
                {


                    if (prisgruppe.Farger == valgtFargeBeholder)
                    {
                        produkt.Prisgruppe = prisgruppe;
                        break;
                    }
                    //if (prisgruppe != null)
                    //    foreach (Farge farge in prisgruppe.Farger.Farger)
                    //    {
                    //        //string s = prisgruppe.Farger.PrisgruppeNavn + ": " + farge;
                    //        if (valgtFargeBeholder.FargeTittel == farge.FargeTittel)
                    //        {
                    //            produkt.Prisgruppe = prisgruppe;
                    //            doBreak = true;
                    //            break;
                    //        }
                    //    }

                    //if (doBreak)
                    //    break;
                }//Prisgrupper



            }//Liste
        }

        void LeggTilKnapp_Click(object sender, RoutedEventArgs e)
        {
            //object o = listview.Items;

            //if (Produkt != null && !string.IsNullOrEmpty(cmbFargevelger.Text))
            //    Produkt.ProduktKommentar += " (" + cmbFargevelger.Text + ")";
            ////////////////////if (Produkt != null && !string.IsNullOrEmpty(cmbFargevelger.Text))
            ////////////////////    Produkt.ProduktNavn += " (" + cmbFargevelger.Text + ")";



            if (!_visPrisgruppeVelger)
            {
                var farge = cmbFargevelger2.SelectedItem as Farge;
                if (farge != null)
                {
                    HuskFarge = farge.FargeTittel.Trim().ToLower();
                }
            }
            else
            {
                var prisgruppe = cmbPrisgruppeVelger.SelectedItem as FargeBeholder;

                if (prisgruppe != null)
                {
                    HuskFarge = prisgruppe.PrisgruppeNavn;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            
        }

        RadioButton sistePrisKlikket = null;

        private void PrisRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton knapp = sender as RadioButton;
            if (knapp == null) return;
            sistePrisKlikket = knapp;

            ListViewItem lvitem = Hjelpeklasser.VisualTree.getParentKontroll<ListViewItem>(knapp);
            if (lvitem != null && lvitem.Content is CorianProdukt )
            {
                CorianProdukt cp = lvitem.Content as CorianProdukt;
                if (cp.Prisgruppe == null) return;
                
                TilvalgEnkelPris p = null;

                string navn = "";
                if (cmbFargevelger2.SelectedItem is Farge)
                {
                    //HACK: Hardcoding av alle farger sånn at stålkummene ikke vises med farge.
                    if (cp.Prisgruppe.Farger.PrisgruppeNavn.ToLower().Trim() == "alle farger")
                        navn = cp.Navn; //+ ", " + cp.Prisgruppe.Farger.PrisgruppeNavn;
                    else
                        navn = (cp.Navn + ", " + cp.Prisgruppe.Farger.PrisgruppeNavn + ", " + cmbFargevelger2.SelectedItem as string).Trim().TrimEnd(',');
                    //navn = (cp.Navn + ", " + (cmbFargevelger2.SelectedItem as Farge).FargeBeholderSomFargenErI.PrisgruppeNavn + ", " + cmbFargevelger2.SelectedItem as string).Trim().TrimEnd(',');
                }
                else
                    navn = (cp.Navn + ", " + cmbFargevelger2.SelectedItem as string).Trim().TrimEnd(',');

                if (cp.PrisPer == CorianProdukt.PerPrisType.perAntall)
                    p = new TilvalgEnkelPris(navn, cp.Prisgruppe.Pris);
                else
                    p = new TilvalgEnkelPrisTusendel(navn, cp.Prisgruppe.Pris);
                
                
                p.ProduktKommentar = cp.ProduktKategori + ": ";
                Produkt = p;


                if (cmbFargevelger2.SelectedItem is Farge)
                    Produkt.Merknader3 = (cmbFargevelger2.SelectedItem as Farge).FargeMerknader.ToArray();

                if (Produkt != null)
                    innhold.Content = Produkt.Kontroll;

            }



            if (innhold.Content == null)
                LeggTilKnapp.IsEnabled = false;
            else
                LeggTilKnapp.IsEnabled = true;
        }

        //void button_Loaded(object sender, RoutedEventArgs e)
        //{
        //    RadioButton button = sender as RadioButton;
        //    if (button == null) return;

        //    TextBlock textblock = button.Content as TextBlock;
        //    if (textblock == null) return;

        //    if (string.IsNullOrEmpty(textblock.Text) || textblock.Text == "0")
        //        button.Visibility = System.Windows.Visibility.Hidden;

        //    //button.Loaded -= button_Loaded;

        //}

    }
}
