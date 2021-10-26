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
    [ModelTable("DiarioClinico")]
    public class SDCL_Sez03 : FwModelRow<SDCL_Sez03>
    {
        public SDCL_Sez03() : base()
        {
        }

        [DataMember()]
        public String Sez03 { get; set; }

        [DataMember()]
        public String DataEvento { get; set; }

        [DataMember()]
        public String DataValidazione { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        public String UtenteValidatore { get; set; }

    }

}
