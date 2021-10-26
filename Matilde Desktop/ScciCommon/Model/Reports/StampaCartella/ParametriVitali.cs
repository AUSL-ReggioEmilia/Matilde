using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("ParametriVitali")]
    public class ParametriVitaliBuffer : FwModelBuffer<ParametriVitali>
    {
        public ParametriVitaliBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("ParametriVitali")]
    public class ParametriVitali : FwModelRow<ParametriVitali>
    {
        public ParametriVitali() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(20)]
        public String Sez09 { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataEvento { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataInserimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String TipoParametroVitale { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteRilevatore { get; set; }

    }
}
