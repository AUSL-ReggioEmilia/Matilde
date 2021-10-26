using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class TipoParametroVitale
    {


        public TipoParametroVitale()
        {
            Codice = "";
            Descrizione = "";
            CodScheda = "";
            DimensioniPerGrafico = new List<DimensionePerGrafico>();
            DimensioniPerFUT = new List<DimensionePerFUT>();
        }

        public List<DimensionePerGrafico> DimensioniPerGrafico { get; set; }
        public List<DimensionePerFUT> DimensioniPerFUT { get; set; }

        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public string CodScheda { get; set; }

    }

}
