using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("PrescrizioniFarmaci")]
    public class PrescrizioniFarmaciBuffer : FwModelBuffer<PrescrizioniFarmaci>
    {
        public PrescrizioniFarmaciBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("PrescrizioniFarmaci")]
    public class PrescrizioniFarmaci : FwModelRow<PrescrizioniFarmaci>
    {
        public PrescrizioniFarmaci() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(25)]
        public String Sez10 { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataValidazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(132)]
        public String DataSospensione { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String TipoPrescrizione { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteValidatore { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        public String DataSomministrazione { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(2000)]
        public String Posologia { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(255)]
        public String ViaSomministrazione { get; set; }

    }
}
