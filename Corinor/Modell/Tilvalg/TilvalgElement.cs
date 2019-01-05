using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corinor.Modell.Tilvalg
{
    [Serializable]
    public class TilvalgElement
    {
        public string TilvalgTittel { get; set; }
        public double Pris { get; set; }
        public double MinstePris { get; set; }
        public PrisBeregningType PrisTypeBeregning { get; set; }

        public int PrisBeregningTypeInt
        {
            get { return (int)PrisTypeBeregning; }
            set
            {
                if (value == 1) PrisTypeBeregning = PrisBeregningType.noramlTusendel;
                else if (value == 2) PrisTypeBeregning = PrisBeregningType.kvadratmeter;
                else if (value == 3) PrisTypeBeregning = PrisBeregningType.totalsumXprosent;
                else if (value == 4) PrisTypeBeregning = PrisBeregningType.egendefinert;
                else PrisTypeBeregning = PrisBeregningType.normal;
            }

        }

        public enum PrisBeregningType
        { 
            normal = 0,
            noramlTusendel = 1,
            kvadratmeter = 2,
            totalsumXprosent = 3,
            egendefinert = 4,

        }

        public TilvalgElement(string tilvalgTittel, double pris, PrisBeregningType prisTypeBeregning, double minstePris)
        {
            this.TilvalgTittel = tilvalgTittel;
            this.Pris = pris;
            this.PrisTypeBeregning = prisTypeBeregning;
            this.MinstePris = minstePris;
        }
    }
}
