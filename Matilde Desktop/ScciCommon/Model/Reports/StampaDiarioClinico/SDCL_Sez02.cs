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
    [ModelTable("TipodiDiario")]
    public class SDCL_Sez02 : FwModelRow<SDCL_Sez02>
    {
        public SDCL_Sez02() : base()
        {
        }

        [DataMember()]
        public String Sez02 { get; set; }

        [DataMember()]
        public String TipoDiario { get; set; }

    }

}
