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
using Corinor.Kontrollbehandling;
using Corinor.Presentasjon;
using Corinor.Hjelpeklasser;
using Corinor.Modell.Heltre;

namespace Corinor.Vinduer
{
    /// <summary>
    /// Interaction logic for NyHeltreplateVindu.xaml
    /// </summary>
    public partial class HeltreplateVindu : Window
    {
        HeltreplateService heltreplateService;
        public HeltreplatePresentasjon HeltreplateSomEndres { get; private set; }
        public string HuskTreslag = "";

        public HeltreplateVindu(DataAksess2 db, string huskTreslag)
        {
            InitializeComponent();
            this.HuskTreslag = huskTreslag;
            heltreplateService = new HeltreplateService(db);
            this.Loaded += Window_Loaded;

            LeggTilKnapp.Click += new RoutedEventHandler(LeggTilKnapp_Click);
            avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);

            løpemeterTekstBoks.TextChanged += løpemeterTekstBoks_TextChanged;
            tykkelseKombo.SelectionChanged += new SelectionChangedEventHandler(tykkelseKombo_SelectionChanged);
            benkeplateTypeKombo.SelectionChanged += new SelectionChangedEventHandler(benkeplateTypeKombo_SelectionChanged);
            treslagKombo.SelectionChanged += new SelectionChangedEventHandler(treslagKombo_SelectionChanged);
            dybdeKombo.SelectionChanged += new SelectionChangedEventHandler(dybdeKombo_SelectionChanged);

        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            KontrollHjelper.oppdaterKombo(tykkelseKombo, heltreplateService.getTykkelser());
            if (tykkelseKombo.Items.Count > 0) tykkelseKombo.SelectedIndex = 0;
            //KontrollHjelper.oppdaterKombo(benkeplateTypeKombo, heltreplateService.getBenkeplateTyper());
        }

        void LeggTilKnapp_Click(object sender, RoutedEventArgs e)
        {
            string tykkelse = tykkelseKombo.SelectedItem as string;
            string treslag = treslagKombo.SelectedItem as string;
            string benkeplateType = benkeplateTypeKombo.SelectedItem as string;
            string størrelse = dybdeKombo.SelectedItem as string;
            int lengde = 0;
            int.TryParse(løpemeterTekstBoks.Text, out lengde);

            double pris = heltreplateService.getPrisPerEnhet(tykkelse, treslag, benkeplateType, størrelse, lengde); ;
            //double.TryParse(prisLabel.Content.ToString(), out pris);
            
            HeltreProdukt.PerPrisType perPris = heltreplateService.getBenkeplatePtype(tykkelse, treslag, benkeplateType, størrelse);

            bool perLøpemeter = false;
            if (perPris == HeltreProdukt.PerPrisType.perLøpemeter) perLøpemeter = true;

            

            HeltreplateSomEndres = new HeltreplatePresentasjon(treslag, benkeplateType, tykkelse, størrelse, lengde, pris, perLøpemeter);
            DialogResult = true;

            this.Close();
        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        void benkeplateTypeKombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedItem == null || !(cmb.SelectedItem is string))
                return;

            KontrollHjelper.oppdaterKombo(treslagKombo, heltreplateService.getTreslag((tykkelseKombo.SelectedItem as string), benkeplateTypeKombo.SelectedItem as string));
            if (treslagKombo.Items.Count > 0)
            {
                var valgt = treslagKombo.Items.Cast<string>().Where(x=>x == HuskTreslag).FirstOrDefault();

                if (valgt != null)
                    treslagKombo.SelectedItem = valgt;
                else
                    treslagKombo.SelectedIndex = 0;
            }

            oppdaterPris();
        }

        void tykkelseKombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tykkelseKombo.SelectedItem == null || !(tykkelseKombo.SelectedItem is string))
                return;
          
            string tykkelse = tykkelseKombo.SelectedItem as string;
            KontrollHjelper.oppdaterKombo(benkeplateTypeKombo, heltreplateService.getBenkeplateTyper(tykkelse));
            if (benkeplateTypeKombo.Items.Count > 0) benkeplateTypeKombo.SelectedIndex = 0;

            oppdaterPris();
        }

        void dybdeKombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            oppdaterPris();

            string tykkelse = tykkelseKombo.SelectedItem as string;
            string treslag = treslagKombo.SelectedItem as string;
            string benkeplate = benkeplateTypeKombo.SelectedItem as string;
            string størrelse = dybdeKombo.SelectedItem as string;

            HeltreProdukt.PerPrisType perPris = heltreplateService.getBenkeplatePtype(tykkelse, treslag, benkeplate, størrelse);

            bool perLøpemeter = false;
            if (perPris == HeltreProdukt.PerPrisType.perLøpemeter) perLøpemeter = true;

            if (perLøpemeter)
            {
                løpemeterLabel.Content = "Løpemeter (mm)";
                løpemeterTekstBoks.Text = "1000";
            }
            else
            {
                løpemeterLabel.Content = "Antall";
                løpemeterTekstBoks.Text = "1";
            }

            løpemeterTekstBoks.IsEnabled = true;


        }

        void treslagKombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedItem == null || !(cmb.SelectedItem is string))
                return;

            KontrollHjelper.oppdaterKombo(dybdeKombo, 
                heltreplateService.getStørrelser((tykkelseKombo.SelectedItem as string), benkeplateTypeKombo.SelectedItem as string, treslagKombo.SelectedItem as string));
            if (dybdeKombo.Items.Count > 0) dybdeKombo.SelectedIndex = 0;

            oppdaterPris();

            HuskTreslag = treslagKombo.SelectedItem as string;
        }

        void løpemeterTekstBoks_TextChanged(object sender, TextChangedEventArgs e)
        {
            oppdaterPris();
        }

    

        private void oppdaterPris()
        {
            string tykkelse = tykkelseKombo.SelectedItem as string;
            string treslag = treslagKombo.SelectedItem as string;
            string benkeplate = benkeplateTypeKombo.SelectedItem as string;
            string størrelse = dybdeKombo.SelectedItem as string;
            int lengde = 0;
            int.TryParse(løpemeterTekstBoks.Text, out lengde);

            //Skal vi her sjekke på prisPer?
            //heltreplateService.getBenkeplatePtype(tykkelse, treslag, benkeplate, størrelse);

            double pris = heltreplateService.getPris(tykkelse, treslag, benkeplate, størrelse, lengde);
            prisLabel.Content = pris;
            
            if (pris > 0)
                LeggTilKnapp.IsEnabled = true;
            else
                LeggTilKnapp.IsEnabled = false;
        }

    }
}
