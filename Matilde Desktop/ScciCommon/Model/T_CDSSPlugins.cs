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
    [ModelTable("T_CDSSPlugins")]
    public class T_CDSSPluginsBuffer : FwModelBuffer<T_CDSSPlugins>
    {
        public T_CDSSPluginsBuffer() : base(Database.ConnectionString)
        {
        }

    }


    [DataContract()]
    [ModelTable("T_CDSSPlugins")]
    public class T_CDSSPlugins : FwModelRow<T_CDSSPlugins>
    {
        public T_CDSSPlugins() : base(Database.ConnectionString)
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
        public String NomePlugin { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Comando { get; set; }

        [DataMember()]
        [ValidationStringLenght(5)]
        public String Modalita { get; set; }

        [DataMember()]
        public Int32? Ordine { get; set; }

        [DataMember()]
        public UniFwByteArray Icona { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodTipoCDSS { get; set; }

    }
}
