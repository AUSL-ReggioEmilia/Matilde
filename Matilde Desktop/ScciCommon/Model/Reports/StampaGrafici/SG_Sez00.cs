using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;

namespace UnicodeSrl.Scci.Model
{

    [DataContract()]
    [ModelTable("DatiGenerali")]
    public class SG_Sez00 : FwModelRow<SG_Sez00>
    {
        public SG_Sez00() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(17)]
        public String Sez00 { get; set; }

        [DataMember()]
        public String IntestazioneStampa { get; set; }

        [DataMember()]
        public String IntestazioneStampaSintetica { get; set; }

        [DataMember()]
        public String IntestazioneCartellaReparto { get; set; }

        [DataMember()]
        public String IntestazioneCartellaRepartoSintetica { get; set; }

        [DataMember()]
        [ValidationStringLenght(1000)]
        public String Titolo { get; set; }

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
        [ValidationStringLenght(50)]
        public String DescrizioneStatoCartella { get; set; }

        [DataMember()]
        public String FirmaCartella { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String NumeroNosologico { get; set; }

    }

}
