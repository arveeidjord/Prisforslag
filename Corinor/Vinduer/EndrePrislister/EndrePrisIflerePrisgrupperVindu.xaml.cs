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
using Corinor.DataAksess;

namespace Corinor.Vinduer.EndrePrislister
{
    /// <summary>
    /// Interaction logic for EndrePrisIflerePrisgrupperVindu.xaml
    /// </summary>
    public partial class EndrePrisIflerePrisgrupperVindu : Window
    {
        public IList<PrisgruppeItem> Prisgrupper { get; set; }
        IList<CorianProdukt> Produkter { get; set; }
        FargeBeholder _valgtPrisgruppe;
        DataAksess2 db;

        public EndrePrisIflerePrisgrupperVindu(IList<CorianProdukt> produkter, FargeBeholder valgtPrisgruppe, DataAksess2 db)
        {
            InitializeComponent();
            this.db = db;

            Prisgrupper = new List<PrisgruppeItem>();
            foreach (var f in db.Produktbeholder.Prisgruppesamling)
            {
                if (f.Visible && f.PrisgruppeNavn != valgtPrisgruppe.PrisgruppeNavn)
                    Prisgrupper.Add(new PrisgruppeItem { Prisgruppe = f });
            }

            _valgtPrisgruppe = valgtPrisgruppe;

            //foreach (CorianProdukt produkt in produkter)
            //{
            //    foreach (CorianPrisgruppeProdukt prisgruppe in produkt.Prisgrupper)
            //    {
            //        if (prisgruppe != null && prisgruppe.Farger.Visible)
            //        {
            //            if (!Prisgrupper.Contains(prisgruppe.Farger))
            //                Prisgrupper.Add(prisgruppe.Farger);
            //        }

            //    }
            //}

            prisgruppeListe.ItemsSource = Prisgrupper;

            this.Produkter = produkter;
        }

        private void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lagreKnapp_Click(object sender, RoutedEventArgs e)
        {
             //TODO: Bare endre pris på de som ikke har avhengigheter?

            var merketPrisgrupper = Prisgrupper.Where(x => x.Merket).ToList();

            foreach (var p in Produkter)
            {
                var valgtProduktPrisgruppe = p.Prisgrupper.FirstOrDefault(x=>x.Farger.PrisgruppeNavn == _valgtPrisgruppe.PrisgruppeNavn);

                if (valgtProduktPrisgruppe != null)
                {
                    foreach (var prisgruppe in p.Prisgrupper)
                    {
                        if (prisgruppe.Farger.PrisgruppeNavn != _valgtPrisgruppe.PrisgruppeNavn
                            && merketPrisgrupper.Count(x=>x.Prisgruppe.PrisgruppeNavn == prisgruppe.Farger.PrisgruppeNavn) > 0)
                        {
                            prisgruppe.Prisgrunnlag = valgtProduktPrisgruppe.Prisgrunnlag;
                        }
                    }
                }
                
                
            }

            this.Close();
        }
    }

    public class PrisgruppeItem
    {
        public FargeBeholder Prisgruppe { get; set; }
        public bool Merket { get; set; }
    }
}
