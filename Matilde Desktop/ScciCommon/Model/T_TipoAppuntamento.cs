using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    [DataContract()]
    [ModelTable("T_TipoAppuntamento")]
    public class T_TipoAppuntamento : FwModelRow<T_TipoAppuntamento>
    {
        public T_TipoAppuntamento() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String Codice { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String Colore { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodScheda { get; set; }

        [DataMember()]
        public Int32? TimeSlotInterval { get; set; }

        [DataMember()]
        [ValidationStringLenght(2000)]
        public String FormulaTitolo { get; set; }

    }
}
