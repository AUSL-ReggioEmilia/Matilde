using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("Allegati")]
    public class AllegatiBuffer : FwModelBuffer<Allegati>
    {
        public AllegatiBuffer() : base()
        {
        }

    }


    [DataContract()]
    [ModelTable("Allegati")]
    public class Allegati : FwModelRow<Allegati>
    {
        public Allegati() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(13)]
        public String Sez13 { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String FormatoAllegato { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String TipoAllegato { get; set; }

        [DataMember()]
        [ValidationStringLenght(26)]
        public String DataInserimento { get; set; }

        [DataMember()]
        public String TestoRTF { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String UtenteInserimento { get; set; }

    }
}
