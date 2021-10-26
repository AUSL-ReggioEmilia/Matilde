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
    [ModelTable("AB_Sez02")]
    public class AB_Sez02 : FwModelRow<AB_Sez02>
    {
        public AB_Sez02() : base()
        {
        }

        [DataMember()]
        public String Sez02 { get; set; }

        [DataMember()]
        public Guid? IDScheda { get; set; }

        [DataMember()]
        public Int32? AnteprimaRTF { get; set; }

        [DataMember()]
        public String DescrUtenteUltimaModifica { get; set; }

        [DataMember()]
        public String DataUltimaModifica { get; set; }

        [DataMember()]
        public Int32? IDTerapia { get; set; }

    }

}
