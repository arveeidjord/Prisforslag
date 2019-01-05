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
using Corinor.Modell.Tilvalg;

namespace Corinor.Vinduer.EndrePrislister
{
    /// <summary>
    /// Interaction logic for EndreTilvalg.xaml
    /// </summary>
    public partial class EndreTilvalgVindu : Window
    {
        public ObservableCollection<TilvalgGruppe> TilvalgGruppeListe { get; private set; }
        public ObservableCollection<TilvalgElement> TilvalgListe { get; private set; }

        DataAksess2 db;

        public EndreTilvalgVindu(DataAksess2 db)
        {
            InitializeComponent();

            this.db = db;
            this.Closing += new System.ComponentModel.CancelEventHandler(EndreTilvalgVindu_Closing);

            this.slettKnapp.Click += new RoutedEventHandler(slettKnapp_Click);
            this.nyTilvalgGruppe.Click += new RoutedEventHandler(nyTilvalgGruppe_Click);
            this.endreTilvalggruppeTittelKnapp.Click += new RoutedEventHandler(endreTilvalggruppeTittelKnapp_Click);

            oppKnapp.Click += new RoutedEventHandler(oppKnapp_Click);
            nedKnapp.Click += new RoutedEventHandler(nedKnapp_Click);

            this.slettTilvalgKnapp.Click += new RoutedEventHandler(slettTilvalgKnapp_Click);
            this.nyttTilvalgKnapp.Click += new RoutedEventHandler(nyttTilvalgKnapp_Click);

            this.avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);
            this.lagreKnapp.Click += new RoutedEventHandler(lagreKnapp_Click);

            TilvalgGruppeListe = db.Produktbeholder.Tilvalgliste;
            if (TilvalgGruppeListe == null)
                TilvalgGruppeListe = new ObservableCollection<TilvalgGruppe>();

            tilvalggruppeListBox.SelectionChanged += new SelectionChangedEventHandler(tilvalggruppeListBox_SelectionChanged);
            tilvalggruppeListBox.DataContext = this;


            if (TilvalgListe == null)
                TilvalgListe = new ObservableCollection<TilvalgElement>();

            tilvalgListbox.DataContext = this;

            
        }

     
        void oppKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (!(tilvalggruppeListBox.SelectedItem is TilvalgGruppe)) return;

            if (tilvalggruppeListBox.SelectedIndex > 0)
                TilvalgGruppeListe.Move(tilvalggruppeListBox.SelectedIndex, tilvalggruppeListBox.SelectedIndex - 1);

            tilvalggruppeListBox.ScrollIntoView(tilvalggruppeListBox.SelectedItem as TilvalgGruppe);
        }

        void nedKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (!(tilvalggruppeListBox.SelectedItem is TilvalgGruppe)) return;

            if (tilvalggruppeListBox.SelectedIndex < tilvalggruppeListBox.Items.Count - 1)
                TilvalgGruppeListe.Move(tilvalggruppeListBox.SelectedIndex, tilvalggruppeListBox.SelectedIndex + 1);

            tilvalggruppeListBox.ScrollIntoView(tilvalggruppeListBox.SelectedItem as TilvalgGruppe);
        }


        void endreTilvalggruppeTittelKnapp_Click(object sender, RoutedEventArgs e)
        {

            if (tilvalggruppeListBox.SelectedItem == null || !(tilvalggruppeListBox.SelectedItem is TilvalgGruppe)) 
                return;

            string nåGruppeTittel = (tilvalggruppeListBox.SelectedItem as TilvalgGruppe).GruppeTittel;
            string gruppeTittel = Microsoft.VisualBasic.Interaction.InputBox("Skriv inn tittel på tilvalggruppen", "Corinor prisforslag", nåGruppeTittel);
            if (!string.IsNullOrEmpty(gruppeTittel))
                (tilvalggruppeListBox.SelectedItem as TilvalgGruppe).GruppeTittel = gruppeTittel;

            //HACK: GruppeTittelen blir i GUI-et blir oppdatert med å endre datacontext
            tilvalggruppeListBox.DataContext = null;
            tilvalggruppeListBox.DataContext = this;
        }

        void nyttTilvalgKnapp_Click(object sender, RoutedEventArgs e)
        {
            TilvalgListe.Add(new TilvalgElement("Tittel", 0, TilvalgElement.PrisBeregningType.normal, 0));
        }

        void slettTilvalgKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (tilvalgListbox.SelectedItem != null && tilvalgListbox.SelectedItem is TilvalgElement)
                TilvalgListe.Remove(tilvalgListbox.SelectedItem as TilvalgElement);
        }

        void tilvalggruppeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tilvalggruppeListBox.SelectedItem == null || !(tilvalggruppeListBox.SelectedItem is TilvalgGruppe))
                return;


            TilvalgListe = (tilvalggruppeListBox.SelectedItem as TilvalgGruppe).tilvalgListe;
            tilvalgKontroll.Visibility = System.Windows.Visibility.Visible;

            tilvalgListbox.DataContext = null;
            tilvalgListbox.DataContext = this;
        }

        void EndreTilvalgVindu_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lagreKnapp.Focus(); //for at alle data skal bli oppdatert

            if (this.DialogResult != true)
            {
                MessageBoxResult res = MessageBox.Show("Vil du lagre endringene i prislisten?", "Corinor prisforslag", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                    db.Produktbeholder.saveData();
            }
        }

        void lagreKnapp_Click(object sender, RoutedEventArgs e)
        {
            lagreKnapp.Focus(); //for at alle data skal bli oppdatert
            db.Produktbeholder.saveData();
            this.DialogResult = true;
            this.Close();
        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void nyTilvalgGruppe_Click(object sender, RoutedEventArgs e)
        {
            string gruppeTittel = Microsoft.VisualBasic.Interaction.InputBox("Skriv inn tittel på tilvalggruppen", "Corinor prisforslag", "");
            if (!string.IsNullOrEmpty(gruppeTittel))
            {
                TilvalgGruppe tilvalgGruppe = new TilvalgGruppe(gruppeTittel, Modell.Produkt.DelingType.Begge);
                tilvalgGruppe.tilvalgListe.Add(new TilvalgElement(gruppeTittel, 0, TilvalgElement.PrisBeregningType.normal, 0));
                TilvalgGruppeListe.Add(tilvalgGruppe);
                tilvalggruppeListBox.SelectedItem = tilvalgGruppe;
                tilvalggruppeListBox.ScrollIntoView(tilvalgGruppe);
            }
        }

        void slettKnapp_Click(object sender, RoutedEventArgs e)
        {
            if (tilvalggruppeListBox.SelectedItem != null && tilvalggruppeListBox.SelectedItem is TilvalgGruppe)
                TilvalgGruppeListe.Remove(tilvalggruppeListBox.SelectedItem as TilvalgGruppe);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
