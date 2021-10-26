using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore
{
    [Serializable()]
    public class ModelloTempoManuale
    {
        #region Property

        public DateTime DataOraInizioOriginale { get; set; }

        public DateTime DataOraFineOriginale { get; set; }

        public double DeltaDataOraInizio { get; set; }

        public double DeltaDataOraFine { get; set; }

        public string NomeProtocollo { get; set; }

        public string EtichettaTempo { get; set; }

        #endregion
    }
}
