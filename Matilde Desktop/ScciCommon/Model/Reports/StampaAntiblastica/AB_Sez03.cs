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
    [ModelTable("AB_Sez03")]
    public class AB_Sez03 : FwModelRow<AB_Sez03>
    {
        public AB_Sez03() : base()
        {
        }

        [DataMember()]
        public String Sez03 { get; set; }

        [DataMember()]
        public String DataInizioterapia { get; set; }

        [DataMember()]
        public String MedicoPrescrittore { get; set; }

        [DataMember()]
        public String Giorno { get; set; }

    }

}
