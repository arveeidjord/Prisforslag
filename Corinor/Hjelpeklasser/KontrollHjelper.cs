using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Corinor.Hjelpeklasser
{
    public class KontrollHjelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kombo">Feks: En kombobox eller Listbox</param>
        /// <param name="liste"></param>
        public static void oppdaterKombo(Selector kombo, string[] liste)
        {
            if (kombo == null || liste == null) return;

            string selectedItem = kombo.SelectedItem as string;
            kombo.Items.Clear();


            foreach (string s in liste)
                kombo.Items.Add(s);

            kombo.IsEnabled = true;

            //Selecter den som allerede var i lista fra før hvis den finnes
            if (!string.IsNullOrEmpty(selectedItem) && kombo.Items.Contains(selectedItem))
                kombo.SelectedItem = selectedItem;


        }

    }
}
