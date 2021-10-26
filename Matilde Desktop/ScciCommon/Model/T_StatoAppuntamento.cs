using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    [ModelTable("T_StatoAppuntamento")]
    public class T_StatoAppuntamentoBuffer : FwModelBuffer<T_StatoAppuntamento>
    {
        public T_StatoAppuntamentoBuffer() : base(Database.ConnectionString)
        {
        }

    }

    [DataContract()]
    [ModelTable("T_StatoAppuntamento")]
    public class T_StatoAppuntamento : FwModelRow<T_StatoAppuntamento>
    {
        public T_StatoAppuntamento() : base(Database.ConnectionString)
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
        public Int32? Ordine { get; set; }

        [DataMember()]
        public UniFwByteArray Icona { get; set; }

        [DataMember]
        public System.Boolean? Riservato { get; set; }

    }

}
