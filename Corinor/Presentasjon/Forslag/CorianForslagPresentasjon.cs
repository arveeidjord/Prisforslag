using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corinor.Presentasjon.Forslag
{
    public class CorianForslagPresentasjon : ForslagPresentasjon
    {
        static int antallCorianForslag = 0;


        public CorianForslagPresentasjon()
            : base(string.Format("Coiran prisforslag {0}", ++antallCorianForslag))
        { 
        
        
        }

    }
}
