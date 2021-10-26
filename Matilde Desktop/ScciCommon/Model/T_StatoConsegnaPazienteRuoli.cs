using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    [DataContract()]
    [ModelTable("T_StatoConsegnaPazienteRuoli")]
    public class T_StatoConsegnaPazienteRuoli : FwModelRow<T_StatoConsegnaPazienteRuoli>
    {

        public T_StatoConsegnaPazienteRuoli() : base(Database.ConnectionString)
        {
        }

        public T_StatoConsegnaPazienteRuoli(string codice) : this()
        {
            this.Codice = codice;
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
        public UniFwByteArray Icona { get; set; }

    }

}
