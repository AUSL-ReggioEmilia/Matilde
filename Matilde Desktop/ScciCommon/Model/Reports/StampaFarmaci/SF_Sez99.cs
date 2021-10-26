using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;

namespace UnicodeSrl.Scci.Model
{

    [DataContract()]
    [ModelTable("Output")]
    public class SF_Sez99 : FwModelRow<SF_Sez99>
    {
        public SF_Sez99() : base()
        {
        }

        [DataMember()]
        public String Sez99 { get; set; }

        [DataMember()]
        public String Risultato { get; set; }

    }

}
