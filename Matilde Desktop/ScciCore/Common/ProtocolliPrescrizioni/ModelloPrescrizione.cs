using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore
{
    [Serializable()]
    public class ModelloPrescrizione
    {
        #region Property

        public Guid ID { get; set; }

        public string Descrizione { get; set; }

        public string CodTipoPrescrizione { get; set; }

        public string CodViaSomministrazione { get; set; }

        public string CodiceScheda { get; set; }

        public int VersioneScheda { get; set; }

        public string DatiXMLScheda { get; set; }

        public List<ModelloPrescrizioneTempo> ModelliPrescrizionitempi { get; set; }

        #endregion
    }
}
