using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{
    public class ProtocolloAttivitaTempo
    {

        public ProtocolloAttivitaTempo()
        {
            this.Codice = string.Empty;
            this.CodiceProtocollo = string.Empty;
            this.Descrizione = string.Empty;

            this.DeltaGiorni = 0;
            this.DeltaOre = 0;
            this.DeltaMinuti = 0;
            this.DeltaAlle00 = false;

            this.TipiTaskInfermieristici = new Dictionary<string, string>();
        }

        public string Codice { get; set; }
        public string CodiceProtocollo { get; set; }
        public string Descrizione { get; set; }

        public int DeltaGiorni { get; set; }
        public int DeltaOre { get; set; }
        public int DeltaMinuti { get; set; }
        public bool DeltaAlle00 { get; set; }

        public Dictionary<string, string> TipiTaskInfermieristici { get; set; }

    }
}
