using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("DiarioClinicoMedico")]
    public class DiarioClinicoMedicoBuffer : FwModelBuffer<DiarioClinicoMedico>
    {
        public DiarioClinicoMedicoBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("DiarioClinicoMedico")]
    public class DiarioClinicoMedico : FwModelRow<DiarioClinicoMedico>
    {
        public DiarioClinicoMedico() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(25)]
        public String Sez07 { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataEvento { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataValidazione { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteValidatore { get; set; }

    }
}
