using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("ElencoAccessi")]
    public class ElencoAccessiBuffer : FwModelBuffer<ElencoAccessi>
    {
        public ElencoAccessiBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("ElencoAccessi")]
    public class ElencoAccessi : FwModelRow<ElencoAccessi>
    {
        public ElencoAccessi() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(18)]
        public String Sez02 { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Movimento { get; set; }

        [DataMember()]
        public DateTime? DataMovimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(30)]
        public String sDataMovimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(1000)]
        public String Descrizione { get; set; }

    }
}
