using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Corinor.Modell.Tilvalg
{
    [Serializable]
    public class TilvalgGruppe : Produkt
    {
        public ObservableCollection<TilvalgElement> tilvalgListe { get; private set; }
        public string GruppeTittel { get; set; }


        public override int DelingInt
        {
            get {
                if (Deling == DelingType.Corian) return 0;
                if (Deling == DelingType.Heltre) return 1;
                if (Deling == DelingType.Begge) return 2;
                else
                {
                    Deling = DelingType.Heltre;
                    return 1;
                }
                
            
            }
            set
            {
                if (value != (int)Deling)
                {
                    if (value == 0) Deling = DelingType.Corian;
                    else if (value == 1) Deling = DelingType.Heltre;
                    else if (value == 2) Deling = DelingType.Begge;
                    else Deling = DelingType.Heltre;
                }
            }

        }


        public TilvalgGruppe(string gruppeTittel, DelingType deling)
            :base(deling)
        {
            tilvalgListe = new ObservableCollection<TilvalgElement>();
            this.GruppeTittel = gruppeTittel;
        }

        public override string ToString()
        {
            return GruppeTittel;
        }
    }
}
