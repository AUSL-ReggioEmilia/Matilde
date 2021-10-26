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
    public class SG_Sez01 : FwModelRow<SG_Sez01>
    {
        public SG_Sez01() : base()
        {
        }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(12)]
        public String Sez01 { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(255)]
        public String Cognome { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(255)]
        public String Nome { get; set; }

        [DataMember()]
        [ValidationStringLenght(1)]
        public String Sesso { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String DataNascita { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodiceFiscale { get; set; }

        [DataMember()]
        public Int32? EtaAllAccesso { get; set; }

        [DataMember()]
        [ValidationStringLenght(511)]
        public String LuogoNascita { get; set; }

        [DataMember()]
        [ValidationStringLenght(781)]
        public String LuogoResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(781)]
        public String LuogoDomicilio { get; set; }

        [DataMember()]
        [ValidationStringLenght(8000)]
        public String MedicoCurante { get; set; }

        [DataMember()]
        public String Esenzioni { get; set; }

    }

}
