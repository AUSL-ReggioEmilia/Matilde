using System;
using System.Runtime.Serialization;

namespace UnicodeSrl.Scci.Model
{

    [DataContract]
    public class MSP_SelMovSommGraficoExt
    {
        public Guid IDPrescrizione { get; set; }
        public string Descrizione { get; set; }
        public Decimal IDIcona { get; set; }
        public string ColoreGrafico { get; set; }
        public string CodViaSomministrazione { get; set; }
        public string ViaSomministrazione { get; set; }
        public DateTime DataEvento { get; set; }
    }
}
