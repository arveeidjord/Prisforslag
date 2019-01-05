using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corinor.Modell.CorianProdukt
{
    [Serializable]
    public class Merknad
    {
        public string MerknadTekst { get; set; }
        public string MerknadMerke { get; set; }

        public Merknad(string merknadMerke, string merknadTekst)
        {
            this.MerknadTekst = merknadTekst;
            this.MerknadMerke = merknadMerke;
        }


    }
}
