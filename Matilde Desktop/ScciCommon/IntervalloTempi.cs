using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class IntervalloTempi
    {
        public IntervalloTempi(DateTime dataorainizio, DateTime dataorafine, string nomeprotocollo, string etichettatempo)
        {
            this.DataOraInizio = dataorainizio;
            this.DataOraFine = dataorafine;
            this.NomeProtocollo = nomeprotocollo;
            this.EtichettaTempo = etichettatempo;
        }

        public DateTime DataOraInizio { get; set; }

        public DateTime DataOraFine { get; set; }

        public string NomeProtocollo { get; set; }

        public string EtichettaTempo { get; set; }

    }

}
