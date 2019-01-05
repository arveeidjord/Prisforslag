using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corinor.DataAksess
{
    public class DataAksess2
    {
        public ProduktBeholder Produktbeholder { get; set; }

        public DataAksess2()
        {
            Produktbeholder = new ProduktBeholder();
        }
    }
}
