using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    [DataContract]
    [ModelTable("T_Moduli")]
    public class T_ModuliRow
    {
        [DataMember]
        [ValidationKey]
        [ValidationStringLenght(50)]
        public String Codice { get; set; }

        [DataMember]
        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }

    }
}
