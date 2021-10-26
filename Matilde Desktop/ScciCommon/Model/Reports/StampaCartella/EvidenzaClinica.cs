using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("EvidenzaClinica")]
    public class EvidenzaClinicaBuffer : FwModelBuffer<EvidenzaClinica>
    {
        public EvidenzaClinicaBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("EvidenzaClinica")]
    public class EvidenzaClinica : FwModelRow<EvidenzaClinica>
    {
        public EvidenzaClinica() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(21)]
        public String Sez12 { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String DataEvento { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataVisione { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String TipoReferto { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteVistatore { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(50)]
        public String NumeroReferto { get; set; }

    }
}
