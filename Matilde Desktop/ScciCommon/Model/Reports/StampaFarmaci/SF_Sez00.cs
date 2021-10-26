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
    public class SF_Sez00 : FwModelRow<SF_Sez00>
    {
        public SF_Sez00() : base()
        {
        }

        [DataMember()]
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
        public String Titolo { get; set; }

        [DataMember()]
        public String Regime { get; set; }

        [DataMember()]
        public String UnitaAtomica { get; set; }

        [DataMember()]
        public String UnitaOperativa { get; set; }

        [DataMember()]
        public String Settore { get; set; }

        [DataMember()]
        public String NumeroCartella { get; set; }

        [DataMember()]
        public String DescrizioneStatoCartella { get; set; }

        [DataMember()]
        public String FirmaCartella { get; set; }

        [DataMember()]
        public String NumeroNosologico { get; set; }

    }

}
