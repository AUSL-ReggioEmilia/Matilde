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
    [ModelTable("Paziente")]
    public class SDCL_Sez01 : FwModelRow<SDCL_Sez01>
    {
        public SDCL_Sez01() : base()
        {
        }

        [DataMember()]
        public String Sez01 { get; set; }

        [DataMember()]
        public String Cognome { get; set; }

        [DataMember()]
        public String Nome { get; set; }

        [DataMember()]
        public String Sesso { get; set; }

        [DataMember()]
        public String DataNascita { get; set; }

        [DataMember()]
        public String CodiceFiscale { get; set; }

        [DataMember()]
        public Int32? EtaAllAccesso { get; set; }

        [DataMember()]
        public String LuogoNascita { get; set; }

        [DataMember()]
        public String LuogoResidenza { get; set; }

        [DataMember()]
        public String LuogoDomicilio { get; set; }

        [DataMember()]
        public String MedicoCurante { get; set; }

        [DataMember()]
        public String Esenzioni { get; set; }

    }

}
