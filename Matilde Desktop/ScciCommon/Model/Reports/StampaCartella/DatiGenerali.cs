using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("DatiGenerali")]
    public class DatiGeneraliBuffer : FwModelBuffer<DatiGenerali>
    {
        public DatiGeneraliBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("DatiGenerali")]
    public class DatiGenerali : FwModelRow<DatiGenerali>
    {
        public DatiGenerali() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(17)]
        public String Sez00 { get; set; }

        [DataMember()]
        public String IntestazioneStampa { get; set; }

        [DataMember()]
        public String IntestazioneCartellaReparto { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Regime { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String UnitaAtomica { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String UnitaOperativa { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Settore { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String NumeroCartella { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String DescrizioneStatoCartella { get; set; }

        [DataMember()]
        public String FirmaCartella { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String NumeroNosologico { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodTipoEpisodio { get; set; }

    }
}
