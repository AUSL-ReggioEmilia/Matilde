using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class IntervalloTempiAttivita
    {
        public IntervalloTempiAttivita(DateTime dataorainizio, string descrizioneprotocollo, string descrizionetempo, string codicetask, string descrizionetask)
        {
            this.DataOraInizio = dataorainizio;
            this.DescrizioneProtocollo = descrizioneprotocollo;
            this.DescrizioneTempo = descrizionetempo;
            this.CodiceTask = codicetask;
            this.DescrizioneTask = descrizionetask;
        }

        public DateTime DataOraInizio { get; set; }

        public string DescrizioneProtocollo { get; set; }

        public string DescrizioneTempo { get; set; }

        public string CodiceTask { get; set; }

        public string DescrizioneTask { get; set; }

    }
}
