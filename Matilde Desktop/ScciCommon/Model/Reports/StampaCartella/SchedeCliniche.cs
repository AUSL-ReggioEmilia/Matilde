using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("SchedeCliniche")]
    public class SchedeClinicheBuffer : FwModelBuffer<SchedeCliniche>
    {
        public SchedeClinicheBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("SchedeCliniche")]
    public class SchedeCliniche : FwModelRow<SchedeCliniche>
    {
        public SchedeCliniche() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(19)]
        public String Sez06 { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String NumeroScheda { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String NumeroTotaleSchede { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String DataCreazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(16)]
        public String DataUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String DescrUtenteUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String DescrUtenteValidazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(16)]
        public String DataValidazione { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public Guid IDScheda { get; set; }

        [DataMember()]
        public Guid? IDSchedaPadre { get; set; }

    }
}
