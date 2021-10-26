using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("Appuntamenti")]
    public class AppuntamentiBuffer : FwModelBuffer<Appuntamenti>
    {
        public AppuntamentiBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("Appuntamenti")]
    public class Appuntamenti : FwModelRow<Appuntamenti>
    {
        public Appuntamenti() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(17)]
        public String Sez14 { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String TipoAppuntamento { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String StatoAppuntamento { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataAppuntamento { get; set; }

        [DataMember()]
        [ValidationStringLenght(2000)]
        public String ElencoRisorse { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataInserimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteInserimento { get; set; }

    }
}
