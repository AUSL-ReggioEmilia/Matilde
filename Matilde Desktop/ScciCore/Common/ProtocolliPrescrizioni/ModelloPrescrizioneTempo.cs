using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore
{
    [Serializable()]
    public class ModelloPrescrizioneTempo
    {
        #region Property

        public string CodTipoPrescrizioneTempi { get; set; }

        public string Posologia { get; set; }

        public string CodProtocollo { get; set; }

        public DateTime DataOraInizioOriginale { get; set; }

        public DateTime DataOraFineOriginale { get; set; }

        public double DeltaDataOraInizio { get; set; }

        public double DeltaDataOraFine { get; set; }

        public bool AlBisogno { get; set; }

        public int Durata { get; set; }

        public bool Continuita { get; set; }

        public int PeriodicitaGiorni { get; set; }

        public int PeriodicitaOre { get; set; }

        public int PeriodicitaMinuti { get; set; }

        public bool Manuale { get; set; }

        public string CodiceSchedaPosologia { get; set; }

        public int? VersioneSchedaPosologia { get; set; }

        public string DatiXMLSchedaPosologia { get; set; }

        public List<ModelloTempoManuale> ModelliTempiManuali { get; set; }

        #endregion
    }
}
