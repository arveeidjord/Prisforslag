using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Corinor.Hjelpeklasser
{
    public class GlobaleUrier
    {
        public static string prislisteNavn = "Prisliste.data";

        public static string standardMappe2()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Corinor prisforslag1.2"); ;
        }

        public static string prislistefilUri()
        {
            return Path.Combine(standardMappe2(), prislisteNavn);

        }     
    }
}
