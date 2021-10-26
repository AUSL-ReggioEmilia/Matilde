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
    [ModelTable("T_TipoParametroVitale")]
    public class T_TipoParametroVitale : FwModelRow<T_TipoParametroVitale>
    {
        public T_TipoParametroVitale() : base(Database.ConnectionString)
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
        [ValidationStringLenght(2000)]
        public String CampiFUT { get; set; }

        [DataMember()]
        public String CampiGrafici { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodScheda { get; set; }

        [DataMember()]
        public Int32? Ordine { get; set; }

    }
}
