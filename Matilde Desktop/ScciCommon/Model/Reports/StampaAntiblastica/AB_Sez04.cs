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
    [ModelTable("AB_Sez04")]
    public class AB_Sez04 : FwModelRow<AB_Sez04>
    {
        public AB_Sez04() : base()
        {
        }

        [DataMember()]
        public String Sez04 { get; set; }

        [DataMember()]
        public Guid? IDScheda { get; set; }

        [DataMember()]
        public String DescrUtenteConfermaTask { get; set; }

    }

}
