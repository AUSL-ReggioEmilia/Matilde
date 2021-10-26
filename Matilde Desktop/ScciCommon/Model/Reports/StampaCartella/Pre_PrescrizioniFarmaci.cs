using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("Pre_PrescrizioniFarmaci")]
    public class Pre_PrescrizioniFarmaciBuffer : FwModelBuffer<Pre_PrescrizioniFarmaci>
    {
        public Pre_PrescrizioniFarmaciBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("Pre_PrescrizioniFarmaci")]
    public class Pre_PrescrizioniFarmaci : FwModelRow<Pre_PrescrizioniFarmaci>
    {
        public Pre_PrescrizioniFarmaci() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(29)]
        public String Sez10Pre { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(57)]
        public String Messaggio { get; set; }

    }
}
