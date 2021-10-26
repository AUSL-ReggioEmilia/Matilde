using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("AlertAnamnestici")]
    public class AlertAnamnesticiBuffer : FwModelBuffer<AlertAnamnestici>
    {
        public AlertAnamnesticiBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("AlertAnamnestici")]
    public class AlertAnamnestici : FwModelRow<AlertAnamnestici>
    {
        public AlertAnamnestici() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(21)]
        public String Sez05 { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataEvento { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteInserimento { get; set; }

    }
}
