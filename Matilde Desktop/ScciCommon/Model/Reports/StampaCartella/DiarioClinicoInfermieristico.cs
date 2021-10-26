using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("DiarioClinicoInfermieristico")]
    public class DiarioClinicoInfermieristicoBuffer : FwModelBuffer<DiarioClinicoInfermieristico>
    {
        public DiarioClinicoInfermieristicoBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("DiarioClinicoInfermieristico")]
    public class DiarioClinicoInfermieristico : FwModelRow<DiarioClinicoInfermieristico>
    {
        public DiarioClinicoInfermieristico() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(34)]
        public String Sez08 { get; set; }

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
